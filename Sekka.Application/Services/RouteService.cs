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

    public RouteService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RouteService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
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

        // Simple distance estimation: sum of straight-line segments (placeholder)
        var totalDistance = dto.OrderIds.Count * 2.5; // ~2.5 km avg between stops

        var route = new Route
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            OrderIds = string.Join(",", dto.OrderIds),
            OptimizedSequence = string.Join(",", dto.OrderIds),
            EstimatedTimeMinutes = (int)(totalDistance * 3), // ~3 min per km
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

        route.OrderIds = string.Join(",", orderIds);
        route.OptimizedSequence = route.OrderIds;
        route.TotalDistanceKm = orderIds.Count * 2.5;
        route.EstimatedTimeMinutes = (int)(route.TotalDistanceKm.GetValueOrDefault() * 3);
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

    private async Task<RouteDto> MapRouteToDtoWithOrders(Route route)
    {
        var orderIdStrings = route.OrderIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
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
