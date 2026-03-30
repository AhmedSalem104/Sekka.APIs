using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly ILogger<ColleagueRadarService> _logger;

    public ColleagueRadarService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ColleagueRadarService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<Result<List<NearbyDriverDto>>> GetNearbyAsync(Guid driverId, double latitude, double longitude, double radiusKm)
    {
        _logger.LogInformation("GetNearbyDrivers for driver {DriverId} at ({Lat}, {Lng})", driverId, latitude, longitude);

        // Requires real-time GPS tracking — return empty for now
        return Task.FromResult(Result<List<NearbyDriverDto>>.Success(new List<NearbyDriverDto>()));
    }

    public async Task<Result<HelpRequestDto>> CreateHelpRequestAsync(Guid driverId, CreateHelpRequestDto dto)
    {
        _logger.LogInformation("CreateHelpRequest by driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<FieldAssistanceRequest, Guid>();

        if (!Enum.TryParse<AssistanceType>(dto.HelpType, true, out var assistanceType))
            assistanceType = AssistanceType.Other;

        var request = new FieldAssistanceRequest
        {
            Id = Guid.NewGuid(),
            RequestingDriverId = driverId,
            Type = assistanceType,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Message = dto.Description ?? dto.Title,
            Status = AssistanceStatus.Pending
        };

        await repo.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        return Result<HelpRequestDto>.Success(MapToDto(request));
    }

    public async Task<Result<List<HelpRequestDto>>> GetNearbyHelpRequestsAsync(Guid driverId, double latitude, double longitude, double radiusKm)
    {
        _logger.LogInformation("GetNearbyHelpRequests for driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<FieldAssistanceRequest, Guid>();
        var spec = new PendingHelpRequestsSpec();
        var requests = await repo.ListAsync(spec);

        // Simple distance filter
        var nearby = requests
            .Where(r => r.RequestingDriverId != driverId)
            .Where(r => CalculateDistanceKm(latitude, longitude, r.Latitude, r.Longitude) <= radiusKm)
            .Select(MapToDto)
            .ToList();

        return Result<List<HelpRequestDto>>.Success(nearby);
    }

    public async Task<Result<HelpRequestDto>> RespondToHelpRequestAsync(Guid driverId, Guid requestId)
    {
        _logger.LogInformation("RespondToHelpRequest {RequestId} by driver {DriverId}", requestId, driverId);

        var repo = _unitOfWork.GetRepository<FieldAssistanceRequest, Guid>();
        var request = await repo.GetByIdAsync(requestId);

        if (request is null)
            return Result<HelpRequestDto>.NotFound("طلب المساعدة غير موجود");

        if (request.Status != AssistanceStatus.Pending)
            return Result<HelpRequestDto>.BadRequest("طلب المساعدة ليس في حالة انتظار");

        request.AssistingDriverId = driverId;
        request.Status = AssistanceStatus.Accepted;
        request.AcceptedAt = DateTime.UtcNow;
        repo.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return Result<HelpRequestDto>.Success(MapToDto(request));
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

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<HelpRequestDto>>> GetMyHelpRequestsAsync(Guid driverId)
    {
        _logger.LogInformation("GetMyHelpRequests for driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<FieldAssistanceRequest, Guid>();
        var spec = new DriverHelpRequestsSpec(driverId);
        var requests = await repo.ListAsync(spec);

        var dtos = requests.Select(MapToDto).ToList();
        return Result<List<HelpRequestDto>>.Success(dtos);
    }

    private static HelpRequestDto MapToDto(FieldAssistanceRequest r) => new()
    {
        Id = r.Id,
        DriverId = r.RequestingDriverId,
        DriverName = string.Empty, // Would need driver lookup
        Title = r.Message ?? r.Type.ToString(),
        Description = r.Message,
        Latitude = r.Latitude,
        Longitude = r.Longitude,
        HelpType = r.Type.ToString(),
        Status = r.Status.ToString(),
        ResponderId = r.AssistingDriverId,
        ResponderName = null,
        CreatedAt = r.CreatedAt,
        ResolvedAt = r.ResolvedAt
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

internal class PendingHelpRequestsSpec : BaseSpecification<FieldAssistanceRequest>
{
    public PendingHelpRequestsSpec()
    {
        SetCriteria(r => r.Status == AssistanceStatus.Pending);
        SetOrderByDescending(r => r.CreatedAt);
    }
}

internal class DriverHelpRequestsSpec : BaseSpecification<FieldAssistanceRequest>
{
    public DriverHelpRequestsSpec(Guid driverId)
    {
        SetCriteria(r => r.RequestingDriverId == driverId || r.AssistingDriverId == driverId);
        SetOrderByDescending(r => r.CreatedAt);
    }
}
