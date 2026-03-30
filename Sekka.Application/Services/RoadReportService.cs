using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class RoadReportService : IRoadReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RoadReportService> _logger;

    public RoadReportService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoadReportService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<RoadReportDto>> CreateAsync(Guid driverId, CreateRoadReportDto dto)
    {
        _logger.LogInformation("CreateRoadReport by driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<RoadReport, Guid>();

        var report = new RoadReport
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            Type = dto.Type,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Description = dto.Description,
            Severity = dto.Severity,
            ConfirmationsCount = 0,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddHours(4)
        };

        await repo.AddAsync(report);
        await _unitOfWork.SaveChangesAsync();

        return Result<RoadReportDto>.Success(MapToDto(report));
    }

    public async Task<Result<List<RoadReportDto>>> GetNearbyAsync(double latitude, double longitude, double radiusKm)
    {
        _logger.LogInformation("GetNearbyRoadReports at ({Lat}, {Lng}) radius {Radius}km", latitude, longitude, radiusKm);

        var repo = _unitOfWork.GetRepository<RoadReport, Guid>();
        var spec = new ActiveRoadReportsSpec();
        var reports = await repo.ListAsync(spec);

        // Simple distance filter using Haversine approximation
        var nearby = reports
            .Where(r => CalculateDistanceKm(latitude, longitude, r.Latitude, r.Longitude) <= radiusKm)
            .Select(MapToDto)
            .ToList();

        return Result<List<RoadReportDto>>.Success(nearby);
    }

    public async Task<Result<bool>> ConfirmAsync(Guid driverId, Guid reportId, bool isConfirmed)
    {
        _logger.LogInformation("ConfirmRoadReport {ReportId} by driver {DriverId}, confirmed: {IsConfirmed}", reportId, driverId, isConfirmed);

        var repo = _unitOfWork.GetRepository<RoadReport, Guid>();
        var report = await repo.GetByIdAsync(reportId);

        if (report is null || !report.IsActive)
            return Result<bool>.NotFound("البلاغ غير موجود أو غير نشط");

        // Save confirmation
        var confirmRepo = _unitOfWork.GetRepository<RoadReportConfirmation, Guid>();
        var confirmation = new RoadReportConfirmation
        {
            Id = Guid.NewGuid(),
            ReportId = reportId,
            DriverId = driverId,
            IsConfirmed = isConfirmed
        };
        await confirmRepo.AddAsync(confirmation);

        if (isConfirmed)
        {
            report.ConfirmationsCount++;
            repo.Update(report);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    public async Task<Result<RoadReportDto>> GetByIdAsync(Guid reportId)
    {
        _logger.LogInformation("GetRoadReportById {ReportId}", reportId);

        var repo = _unitOfWork.GetRepository<RoadReport, Guid>();
        var report = await repo.GetByIdAsync(reportId);

        if (report is null)
            return Result<RoadReportDto>.NotFound("البلاغ غير موجود");

        return Result<RoadReportDto>.Success(MapToDto(report));
    }

    public async Task<Result<List<RoadReportDto>>> GetMyReportsAsync(Guid driverId)
    {
        _logger.LogInformation("GetMyRoadReports for driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<RoadReport, Guid>();
        var spec = new DriverRoadReportsSpec(driverId);
        var reports = await repo.ListAsync(spec);

        var dtos = reports.Select(MapToDto).ToList();
        return Result<List<RoadReportDto>>.Success(dtos);
    }

    public async Task<Result<bool>> DeactivateAsync(Guid driverId, Guid reportId)
    {
        _logger.LogInformation("DeactivateRoadReport {ReportId} by driver {DriverId}", reportId, driverId);

        var repo = _unitOfWork.GetRepository<RoadReport, Guid>();
        var report = await repo.GetByIdAsync(reportId);

        if (report is null || report.DriverId != driverId)
            return Result<bool>.NotFound("البلاغ غير موجود");

        report.IsActive = false;
        repo.Update(report);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    private static RoadReportDto MapToDto(RoadReport r) => new()
    {
        Id = r.Id,
        DriverId = r.DriverId,
        Type = r.Type,
        Latitude = r.Latitude,
        Longitude = r.Longitude,
        Description = r.Description,
        Severity = r.Severity,
        ConfirmationsCount = r.ConfirmationsCount,
        IsActive = r.IsActive,
        ExpiresAt = r.ExpiresAt,
        CreatedAt = r.CreatedAt
    };

    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth radius in km
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}

internal class ActiveRoadReportsSpec : BaseSpecification<RoadReport>
{
    public ActiveRoadReportsSpec()
    {
        SetCriteria(r => r.IsActive && r.ExpiresAt > DateTime.UtcNow);
        SetOrderByDescending(r => r.CreatedAt);
    }
}

internal class DriverRoadReportsSpec : BaseSpecification<RoadReport>
{
    public DriverRoadReportsSpec(Guid driverId)
    {
        SetCriteria(r => r.DriverId == driverId);
        SetOrderByDescending(r => r.CreatedAt);
    }
}
