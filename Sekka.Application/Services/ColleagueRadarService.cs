using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class ColleagueRadarService : IColleagueRadarService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFirebaseService _firebaseService;
    private readonly ILogger<ColleagueRadarService> _logger;

    public ColleagueRadarService(IUnitOfWork unitOfWork, IFirebaseService firebaseService, ILogger<ColleagueRadarService> logger)
    {
        _unitOfWork = unitOfWork;
        _firebaseService = firebaseService;
        _logger = logger;
    }

    public async Task<Result<List<NearbyDriverDto>>> GetNearbyAsync(Guid driverId, double latitude, double longitude, double radiusKm)
    {
        _logger.LogInformation("GetNearbyDrivers for driver {DriverId} at ({Lat}, {Lng})", driverId, latitude, longitude);

        var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();
        var cutoff = DateTime.UtcNow.AddMinutes(-30);
        var spec = new OnlineDriversWithLocationSpec(cutoff, driverId);
        var drivers = await driverRepo.ListAsync(spec);

        var nearby = drivers
            .Select(d =>
            {
                var dist = CalculateDistanceKm(latitude, longitude, d.LastKnownLatitude!.Value, d.LastKnownLongitude!.Value);
                return new { Driver = d, Distance = dist };
            })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance)
            .Select(x => new NearbyDriverDto
            {
                DriverId = x.Driver.Id,
                DriverName = x.Driver.Name,
                Latitude = x.Driver.LastKnownLatitude!.Value,
                Longitude = x.Driver.LastKnownLongitude!.Value,
                DistanceKm = Math.Round(x.Distance, 2),
                IsAvailable = x.Driver.IsActive,
                IsOnline = x.Driver.IsOnline,
                VehicleType = x.Driver.VehicleType?.ToString()
            })
            .ToList();

        return Result<List<NearbyDriverDto>>.Success(nearby);
    }

    public async Task<Result<HelpRequestDto>> CreateHelpRequestAsync(Guid driverId, CreateHelpRequestDto dto)
    {
        _logger.LogInformation("CreateHelpRequest by driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<FieldAssistanceRequest, Guid>();
        var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();

        var driver = await driverRepo.GetByIdAsync(driverId);
        if (driver is null)
            return Result<HelpRequestDto>.NotFound("السواق غير موجود");

        var assistanceType = ParseHelpType(dto.HelpType);

        var request = new FieldAssistanceRequest
        {
            Id = Guid.NewGuid(),
            RequestingDriverId = driverId,
            Type = assistanceType,
            Title = dto.Title,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Message = dto.Description ?? dto.Title,
            Status = AssistanceStatus.Pending,
            OrderId = dto.OrderId,
            DriverPhone = dto.DriverPhone
        };

        await repo.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        // بعت push notification لكل السواقين Online في 10 كم
        _ = Task.Run(async () =>
        {
            try
            {
                var cutoff = DateTime.UtcNow.AddMinutes(-30);
                var nearbySpec = new OnlineDriversWithLocationSpec(cutoff, driverId);
                var nearbyDrivers = await driverRepo.ListAsync(nearbySpec);

                var inRange = nearbyDrivers
                    .Where(d => CalculateDistanceKm(dto.Latitude, dto.Longitude, d.LastKnownLatitude!.Value, d.LastKnownLongitude!.Value) <= 10)
                    .Select(d => d.Id)
                    .ToList();

                if (inRange.Any())
                {
                    await _firebaseService.SendPushToManyAsync(
                        inRange,
                        "طلب مساعدة جديد 🆘",
                        $"{driver.Name} يحتاج مساعدة: {dto.Title}",
                        new Dictionary<string, string>
                        {
                            ["type"] = "HELP_REQUEST",
                            ["requestId"] = request.Id.ToString()
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending help request notifications");
            }
        });

        // جهز الـ OrderSummary لو فيه OrderId
        OrderSummaryDto? orderSummary = null;
        if (request.OrderId.HasValue)
            orderSummary = await GetOrderSummaryAsync(request.OrderId.Value);

        return Result<HelpRequestDto>.Success(new HelpRequestDto
        {
            Id = request.Id,
            DriverId = driverId,
            DriverName = driver.Name,
            Title = request.Title,
            Description = request.Message,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            HelpType = request.Type.ToString(),
            Status = request.Status.ToString(),
            DriverPhone = request.DriverPhone,
            OrderId = request.OrderId,
            OrderSummary = orderSummary,
            CreatedAt = request.CreatedAt
        });
    }

    public async Task<Result<List<HelpRequestDto>>> GetNearbyHelpRequestsAsync(Guid driverId, double latitude, double longitude, double radiusKm)
    {
        _logger.LogInformation("GetNearbyHelpRequests for driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<FieldAssistanceRequest, Guid>();
        var spec = new PendingHelpRequestsWithDriverSpec();
        var requests = await repo.ListAsync(spec);

        var nearby = new List<HelpRequestDto>();
        foreach (var r in requests.Where(r => r.RequestingDriverId != driverId))
        {
            var dist = CalculateDistanceKm(latitude, longitude, r.Latitude, r.Longitude);
            if (dist <= radiusKm)
            {
                var dto = await MapToDtoAsync(r, latitude, longitude);
                nearby.Add(dto);
            }
        }

        return Result<List<HelpRequestDto>>.Success(nearby);
    }

    public async Task<Result<HelpRequestDto>> RespondToHelpRequestAsync(Guid driverId, Guid requestId)
    {
        _logger.LogInformation("RespondToHelpRequest {RequestId} by driver {DriverId}", requestId, driverId);

        var repo = _unitOfWork.GetRepository<FieldAssistanceRequest, Guid>();
        var request = await repo.GetByIdAsync(requestId);

        if (request is null)
            return Result<HelpRequestDto>.NotFound("طلب المساعدة غير موجود");

        if (request.RequestingDriverId == driverId)
            return Result<HelpRequestDto>.BadRequest("مش ممكن ترد على طلبك أنت");

        if (request.Status != AssistanceStatus.Pending)
            return Result<HelpRequestDto>.BadRequest("طلب المساعدة ليس في حالة انتظار");

        request.AssistingDriverId = driverId;
        request.Status = AssistanceStatus.Accepted;
        request.AcceptedAt = DateTime.UtcNow;
        repo.Update(request);
        await _unitOfWork.SaveChangesAsync();

        // بعت push notification لصاحب الطلب
        var responderRepo = _unitOfWork.GetRepository<Driver, Guid>();
        var responder = await responderRepo.GetByIdAsync(driverId);
        if (responder is not null)
        {
            await _firebaseService.SendPushAsync(
                request.RequestingDriverId,
                "تم قبول طلب المساعدة ✅",
                $"{responder.Name} في الطريق لمساعدتك",
                new Dictionary<string, string>
                {
                    ["type"] = "HELP_REQUEST_ACCEPTED",
                    ["requestId"] = requestId.ToString()
                });
        }

        var dto = await MapToDtoAsync(request);
        return Result<HelpRequestDto>.Success(dto);
    }

    public async Task<Result<bool>> ResolveHelpRequestAsync(Guid driverId, Guid requestId)
    {
        _logger.LogInformation("ResolveHelpRequest {RequestId} by driver {DriverId}", requestId, driverId);

        var repo = _unitOfWork.GetRepository<FieldAssistanceRequest, Guid>();
        var request = await repo.GetByIdAsync(requestId);

        if (request is null)
            return Result<bool>.NotFound("طلب المساعدة غير موجود");

        if (request.RequestingDriverId != driverId && request.AssistingDriverId != driverId)
            return Result<bool>.BadRequest("ليس لديك صلاحية لحل هذا الطلب");

        request.Status = AssistanceStatus.Resolved;
        request.ResolvedAt = DateTime.UtcNow;
        repo.Update(request);
        await _unitOfWork.SaveChangesAsync();

        // بعت push notification للـ responder
        if (request.AssistingDriverId.HasValue && request.AssistingDriverId.Value != driverId)
        {
            await _firebaseService.SendPushAsync(
                request.AssistingDriverId.Value,
                "تم حل طلب المساعدة ✅",
                "شكراً لمساعدتك! تم حل المشكلة",
                new Dictionary<string, string>
                {
                    ["type"] = "HELP_REQUEST_RESOLVED",
                    ["requestId"] = requestId.ToString()
                });
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<HelpRequestDto>>> GetMyHelpRequestsAsync(Guid driverId)
    {
        _logger.LogInformation("GetMyHelpRequests for driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<FieldAssistanceRequest, Guid>();
        var spec = new DriverHelpRequestsWithDriverSpec(driverId);
        var requests = await repo.ListAsync(spec);

        var dtos = new List<HelpRequestDto>();
        foreach (var r in requests)
            dtos.Add(await MapToDtoAsync(r));

        return Result<List<HelpRequestDto>>.Success(dtos);
    }

    public async Task<Result<bool>> UpdateLocationAsync(Guid driverId, UpdateLocationDto dto)
    {
        _logger.LogInformation("UpdateLocation for driver {DriverId}", driverId);

        var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();
        var driver = await driverRepo.GetByIdAsync(driverId);

        if (driver is null)
            return Result<bool>.NotFound("السواق غير موجود");

        driver.LastKnownLatitude = dto.Latitude;
        driver.LastKnownLongitude = dto.Longitude;
        driver.LastLocationUpdate = DateTime.UtcNow;
        driverRepo.Update(driver);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    // ── Helpers ──────────────────────────────────────────────────

    private async Task<HelpRequestDto> MapToDtoAsync(FieldAssistanceRequest r, double? fromLat = null, double? fromLng = null)
    {
        var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();

        // Driver name lookup
        var requestingDriver = r.RequestingDriver ?? await driverRepo.GetByIdAsync(r.RequestingDriverId);
        var driverName = requestingDriver?.Name ?? string.Empty;

        // Responder lookup
        string? responderName = null;
        string? responderPhone = null;
        if (r.AssistingDriverId.HasValue)
        {
            var responder = r.AssistingDriver ?? await driverRepo.GetByIdAsync(r.AssistingDriverId.Value);
            responderName = responder?.Name;
            responderPhone = responder?.PhoneNumber;
        }

        // Order summary
        OrderSummaryDto? orderSummary = null;
        if (r.OrderId.HasValue)
            orderSummary = await GetOrderSummaryAsync(r.OrderId.Value);

        // Distance
        double? distanceKm = null;
        if (fromLat.HasValue && fromLng.HasValue)
            distanceKm = Math.Round(CalculateDistanceKm(fromLat.Value, fromLng.Value, r.Latitude, r.Longitude), 2);

        return new HelpRequestDto
        {
            Id = r.Id,
            DriverId = r.RequestingDriverId,
            DriverName = driverName,
            Title = r.Title,
            Description = r.Message,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            HelpType = r.Type.ToString(),
            Status = r.Status.ToString(),
            ResponderId = r.AssistingDriverId,
            ResponderName = responderName,
            ResponderPhone = responderPhone,
            DriverPhone = r.DriverPhone,
            DistanceKm = distanceKm,
            OrderId = r.OrderId,
            OrderSummary = orderSummary,
            CreatedAt = r.CreatedAt,
            ResolvedAt = r.ResolvedAt
        };
    }

    private async Task<OrderSummaryDto?> GetOrderSummaryAsync(Guid orderId)
    {
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await orderRepo.GetByIdAsync(orderId);
        if (order is null) return null;

        return new OrderSummaryDto
        {
            OrderNumber = order.OrderNumber,
            CustomerName = order.CustomerName,
            DeliveryAddress = order.DeliveryAddress,
            Amount = order.Amount,
            PaymentMethod = (int)order.PaymentMethod,
            CustomerPhone = order.CustomerPhone
        };
    }

    private static AssistanceType ParseHelpType(string helpType) => helpType?.ToLower() switch
    {
        "mechanical" => AssistanceType.MechanicalIssue,
        "tire" => AssistanceType.FlatTire,
        "fuel" => AssistanceType.FuelEmpty,
        "order" => AssistanceType.OrderDelivery,
        "accident" => AssistanceType.Accident,
        _ => AssistanceType.Other
    };

    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}

// ── Specifications ──────────────────────────────────────────────

internal class OnlineDriversWithLocationSpec : BaseSpecification<Driver>
{
    public OnlineDriversWithLocationSpec(DateTime cutoff, Guid excludeDriverId)
    {
        SetCriteria(d => d.IsOnline
            && d.IsActive
            && d.Id != excludeDriverId
            && d.LastKnownLatitude.HasValue
            && d.LastKnownLongitude.HasValue
            && d.LastLocationUpdate.HasValue
            && d.LastLocationUpdate.Value >= cutoff);
    }
}

internal class PendingHelpRequestsWithDriverSpec : BaseSpecification<FieldAssistanceRequest>
{
    public PendingHelpRequestsWithDriverSpec()
    {
        SetCriteria(r => r.Status == AssistanceStatus.Pending);
        AddInclude(r => r.RequestingDriver);
        SetOrderByDescending(r => r.CreatedAt);
    }
}

internal class DriverHelpRequestsWithDriverSpec : BaseSpecification<FieldAssistanceRequest>
{
    public DriverHelpRequestsWithDriverSpec(Guid driverId)
    {
        SetCriteria(r => r.RequestingDriverId == driverId || r.AssistingDriverId == driverId);
        AddInclude(r => r.RequestingDriver);
        SetOrderByDescending(r => r.CreatedAt);
    }
}

internal class ActiveHelpRequestsSpec : BaseSpecification<FieldAssistanceRequest>
{
    public ActiveHelpRequestsSpec(DateTime pendingCutoff, DateTime acceptedCutoff)
    {
        SetCriteria(r =>
            (r.Status == AssistanceStatus.Pending && r.CreatedAt < pendingCutoff) ||
            (r.Status == AssistanceStatus.Accepted && r.AcceptedAt.HasValue && r.AcceptedAt.Value < acceptedCutoff));
    }
}
