using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Route;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class RouteService : IRouteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RouteService> _logger;
    private readonly IMapDistanceService _mapService;

    public RouteService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RouteService> logger, IMapDistanceService mapService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _mapService = mapService;
    }

    public async Task<Result<RouteDto>> OptimizeRouteAsync(Guid driverId, OptimizeRouteDto dto)
    {
        _logger.LogInformation("Route optimization requested by driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<Route, Guid>();

        // Deactivate any existing active route for this driver
        var activeSpec = new ActiveRouteSpec(driverId);
        var activeRoutes = await repo.ListAsync(activeSpec);
        foreach (var ar in activeRoutes)
        {
            ar.IsActive = false;
            repo.Update(ar);
        }

        // Fetch orders to get their coordinates
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var waypoints = new List<IndexedWaypoint>();
        foreach (var orderId in dto.OrderIds)
        {
            var order = await orderRepo.GetByIdAsync(orderId);
            if (order?.DeliveryLatitude != null && order.DeliveryLongitude != null)
            {
                waypoints.Add(new IndexedWaypoint(
                    order.Id,
                    order.DeliveryLatitude.Value,
                    order.DeliveryLongitude.Value));
            }
        }

        string optimizedSequence;
        double totalDistance;
        int estimatedMinutes;

        // Use ORS Optimization API for real route optimization
        var startPoint = new GeoPoint(dto.StartLatitude, dto.StartLongitude);
        var optimized = await _mapService.OptimizeWaypointsAsync(startPoint, waypoints);

        if (optimized is not null && optimized.OptimizedOrder.Count > 0)
        {
            optimizedSequence = string.Join(",", optimized.OptimizedOrder);
            totalDistance = optimized.TotalDistanceKm;
            estimatedMinutes = (int)Math.Ceiling(optimized.TotalDurationMinutes);
            _logger.LogInformation("ORS optimization: {Distance} km, {Minutes} min for {Count} stops",
                totalDistance, estimatedMinutes, waypoints.Count);

            // Sanity check: max 100 km per stop for local delivery
            var maxReasonable = Math.Max(50, waypoints.Count * 100.0);
            if (totalDistance > maxReasonable)
            {
                _logger.LogWarning("ORS returned unrealistic distance {Distance} km for {Count} stops, capping to Haversine estimate",
                    totalDistance, waypoints.Count);
                totalDistance = EstimateHaversineDistance(dto.StartLatitude, dto.StartLongitude, waypoints);
                estimatedMinutes = (int)(totalDistance * 3);
            }
        }
        else
        {
            // Fallback: estimate from Haversine distances if coordinates available
            optimizedSequence = string.Join(",", dto.OrderIds);
            if (waypoints.Count > 0)
            {
                totalDistance = EstimateHaversineDistance(dto.StartLatitude, dto.StartLongitude, waypoints);
            }
            else
            {
                totalDistance = dto.OrderIds.Count * 2.5;
            }
            estimatedMinutes = (int)(totalDistance * 3);
            _logger.LogWarning("ORS unavailable, using fallback estimate {Distance} km for {Count} stops", totalDistance, dto.OrderIds.Count);
        }

        var route = new Route
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            OrderIds = string.Join(",", dto.OrderIds),
            OptimizedSequence = optimizedSequence,
            EstimatedTimeMinutes = estimatedMinutes,
            TotalDistanceKm = totalDistance,
            StartLatitude = dto.StartLatitude,
            StartLongitude = dto.StartLongitude,
            IsActive = true
        };

        await repo.AddAsync(route);
        await _unitOfWork.SaveChangesAsync();

        return Result<RouteDto>.Success(await MapRouteToDtoWithOrders(route));
    }

    public async Task<Result<RouteDto>> GetActiveRouteAsync(Guid driverId)
    {
        _logger.LogInformation("Active route requested by driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<Route, Guid>();
        var spec = new ActiveRouteSpec(driverId);
        var routes = await repo.ListAsync(spec);
        var route = routes.FirstOrDefault();

        if (route is null)
            return Result<RouteDto>.NotFound("لا يوجد مسار نشط");

        return Result<RouteDto>.Success(await MapRouteToDtoWithOrders(route));
    }

    public async Task<Result<RouteDto>> ReorderRouteAsync(Guid driverId, Guid routeId, ReorderRouteDto dto)
    {
        _logger.LogInformation("Route reorder requested by driver {DriverId} for route {RouteId}", driverId, routeId);

        var repo = _unitOfWork.GetRepository<Route, Guid>();
        var route = await repo.GetByIdAsync(routeId);

        if (route is null || route.DriverId != driverId)
            return Result<RouteDto>.NotFound("المسار غير موجود");

        route.OptimizedSequence = string.Join(",", dto.OrderIds);
        route.OrderIds = string.Join(",", dto.OrderIds);
        repo.Update(route);
        await _unitOfWork.SaveChangesAsync();

        return Result<RouteDto>.Success(await MapRouteToDtoWithOrders(route));
    }

    public async Task<Result<RouteDto>> AddOrderToRouteAsync(Guid driverId, Guid routeId, AddOrderToRouteDto dto)
    {
        _logger.LogInformation("Add order to route requested by driver {DriverId} for route {RouteId}", driverId, routeId);

        var repo = _unitOfWork.GetRepository<Route, Guid>();
        var route = await repo.GetByIdAsync(routeId);

        if (route is null || route.DriverId != driverId)
            return Result<RouteDto>.NotFound("المسار غير موجود");

        var orderIds = route.OrderIds.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        if (dto.InsertAtIndex.HasValue && dto.InsertAtIndex.Value < orderIds.Count)
            orderIds.Insert(dto.InsertAtIndex.Value, dto.OrderId.ToString());
        else
            orderIds.Add(dto.OrderId.ToString());

        // Recalculate distance with ORS if we have coordinates
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var waypoints = new List<IndexedWaypoint>();
        foreach (var oidStr in orderIds)
        {
            if (Guid.TryParse(oidStr, out var oid))
            {
                var order = await orderRepo.GetByIdAsync(oid);
                if (order?.DeliveryLatitude != null && order.DeliveryLongitude != null)
                    waypoints.Add(new IndexedWaypoint(oid, order.DeliveryLatitude.Value, order.DeliveryLongitude.Value));
            }
        }

        if (waypoints.Count > 0 && route.StartLatitude.HasValue && route.StartLongitude.HasValue)
        {
            var matrix = await _mapService.GetDistanceMatrixAsync(
                new List<GeoPoint> { new(route.StartLatitude.Value, route.StartLongitude.Value) }
                    .Concat(waypoints.Select(w => new GeoPoint(w.Latitude, w.Longitude)))
                    .ToList());

            if (matrix is not null)
            {
                // Sum sequential distances: start→1, 1→2, 2→3, ...
                double totalDist = 0, totalDur = 0;
                for (int i = 0; i < waypoints.Count; i++)
                {
                    totalDist += matrix.DistancesKm[i][i + 1];
                    totalDur += matrix.DurationsMinutes[i][i + 1];
                }

                // Sanity check
                var maxReasonable = Math.Max(50, waypoints.Count * 100.0);
                if (totalDist > maxReasonable)
                {
                    _logger.LogWarning("Matrix returned unrealistic distance {Distance} km, using Haversine", totalDist);
                    totalDist = EstimateHaversineDistance(
                        route.StartLatitude ?? 0, route.StartLongitude ?? 0, waypoints);
                    totalDur = totalDist * 3;
                }

                route.TotalDistanceKm = totalDist;
                route.EstimatedTimeMinutes = (int)Math.Ceiling(totalDur);
            }
        }
        else
        {
            // Haversine fallback
            if (waypoints.Count > 0 && route.StartLatitude.HasValue && route.StartLongitude.HasValue)
            {
                route.TotalDistanceKm = EstimateHaversineDistance(
                    route.StartLatitude.Value, route.StartLongitude.Value, waypoints);
            }
            else
            {
                route.TotalDistanceKm = orderIds.Count * 2.5;
            }
            route.EstimatedTimeMinutes = (int)(route.TotalDistanceKm.GetValueOrDefault() * 3);
        }

        route.OrderIds = string.Join(",", orderIds);
        route.OptimizedSequence = route.OrderIds;
        repo.Update(route);
        await _unitOfWork.SaveChangesAsync();

        return Result<RouteDto>.Success(await MapRouteToDtoWithOrders(route));
    }

    public async Task<Result<RouteDto>> CompleteRouteAsync(Guid driverId, Guid routeId)
    {
        _logger.LogInformation("Route completion requested by driver {DriverId} for route {RouteId}", driverId, routeId);

        var repo = _unitOfWork.GetRepository<Route, Guid>();
        var route = await repo.GetByIdAsync(routeId);

        if (route is null || route.DriverId != driverId)
            return Result<RouteDto>.NotFound("المسار غير موجود");

        route.IsActive = false;
        route.CompletedAt = DateTime.UtcNow;
        repo.Update(route);
        await _unitOfWork.SaveChangesAsync();

        return Result<RouteDto>.Success(await MapRouteToDtoWithOrders(route));
    }

    private static double EstimateHaversineDistance(double startLat, double startLon, List<IndexedWaypoint> waypoints)
    {
        double total = 0;
        double curLat = startLat, curLon = startLon;
        foreach (var wp in waypoints)
        {
            total += HaversineKm(curLat, curLon, wp.Latitude, wp.Longitude);
            curLat = wp.Latitude;
            curLon = wp.Longitude;
        }
        return Math.Round(total * 1.3, 2); // 1.3x factor for road vs straight-line
    }

    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
            + Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0)
            * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private async Task<RouteDto> MapRouteToDtoWithOrders(Route route)
    {
        var orderIdStrings = route.OptimizedSequence?.Split(',', StringSplitOptions.RemoveEmptyEntries)
            ?? route.OrderIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();

        var orders = new List<RouteOrderDto>();
        for (int idx = 0; idx < orderIdStrings.Length; idx++)
        {
            if (Guid.TryParse(orderIdStrings[idx], out var oid))
            {
                var order = await orderRepo.GetByIdAsync(oid);
                orders.Add(new RouteOrderDto
                {
                    OrderId = oid,
                    OrderNumber = order?.OrderNumber ?? "N/A",
                    SequenceIndex = idx,
                    CustomerName = order?.CustomerName,
                    DeliveryAddress = order?.DeliveryAddress ?? "",
                    Amount = order?.Amount ?? 0,
                    Status = order?.Status ?? Core.Enums.OrderStatus.Pending
                });
            }
        }

        return new RouteDto
        {
            Id = route.Id,
            EstimatedTimeMinutes = route.EstimatedTimeMinutes,
            TotalDistanceKm = route.TotalDistanceKm,
            EfficiencyScore = route.EfficiencyScore,
            IsActive = route.IsActive,
            Orders = orders
        };
    }
}

internal class ActiveRouteSpec : BaseSpecification<Route>
{
    public ActiveRouteSpec(Guid driverId)
    {
        SetCriteria(r => r.DriverId == driverId && r.IsActive);
        SetOrderByDescending(r => r.CreatedAt);
    }
}
