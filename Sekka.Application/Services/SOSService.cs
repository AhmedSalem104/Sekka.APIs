using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Communication;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class SOSService : ISOSService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<SOSService> _logger;

    public SOSService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SOSService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<SOSLogDto>> ActivateAsync(Guid driverId, ActivateSOSDto dto)
    {
        var repo = _unitOfWork.GetRepository<SOSLog, Guid>();

        var sos = new SOSLog
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Status = SOSStatus.Active,
            Notes = dto.Notes,
            ActivatedAt = DateTime.UtcNow,
            AdminNotified = true,
            LocationSharedWithFamily = true,
            EscalationLevel = SOSEscalationLevel.Normal
        };

        await repo.AddAsync(sos);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogWarning("SOS activated by driver {DriverId} at ({Lat}, {Lng})", driverId, dto.Latitude, dto.Longitude);

        return Result<SOSLogDto>.Success(_mapper.Map<SOSLogDto>(sos));
    }

    public async Task<Result<bool>> DismissAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<SOSLog, Guid>();
        var sos = await repo.GetByIdAsync(id);

        if (sos == null || sos.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.SOSNotFound);

        if (sos.Status != SOSStatus.Active)
            return Result<bool>.BadRequest(ErrorMessages.SOSAlreadyResolved);

        sos.Status = SOSStatus.Dismissed;
        sos.ResolvedAt = DateTime.UtcNow;
        repo.Update(sos);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("SOS {SOSId} dismissed by driver {DriverId}", id, driverId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<SOSLogDto>> ResolveAsync(Guid driverId, Guid id, ResolveSOSDto dto)
    {
        var repo = _unitOfWork.GetRepository<SOSLog, Guid>();
        var sos = await repo.GetByIdAsync(id);

        if (sos == null || sos.DriverId != driverId)
            return Result<SOSLogDto>.NotFound(ErrorMessages.SOSNotFound);

        if (sos.Status != SOSStatus.Active)
            return Result<SOSLogDto>.BadRequest(ErrorMessages.SOSAlreadyResolved);

        sos.Status = dto.WasFalseAlarm ? SOSStatus.FalseAlarm : SOSStatus.Resolved;
        sos.ResolvedAt = DateTime.UtcNow;
        sos.ResolvedBy = driverId.ToString();
        if (dto.Resolution != null)
            sos.Notes = $"{sos.Notes}\n--- Resolution ---\n{dto.Resolution}".Trim();

        repo.Update(sos);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("SOS {SOSId} resolved by driver {DriverId}. FalseAlarm: {FalseAlarm}", id, driverId, dto.WasFalseAlarm);

        return Result<SOSLogDto>.Success(_mapper.Map<SOSLogDto>(sos));
    }

    public async Task<Result<PagedResult<SOSLogDto>>> GetHistoryAsync(Guid driverId, PaginationDto pagination)
    {
        var repo = _unitOfWork.GetRepository<SOSLog, Guid>();
        var spec = new SOSByDriverSpec(driverId, pagination);
        var items = await repo.ListAsync(spec);
        var countSpec = new SOSByDriverCountSpec(driverId);
        var total = await repo.CountAsync(countSpec);

        var dtos = _mapper.Map<List<SOSLogDto>>(items);
        return Result<PagedResult<SOSLogDto>>.Success(
            new PagedResult<SOSLogDto>(dtos, total, pagination.Page, pagination.PageSize));
    }
}

// ── Specifications ──

internal class SOSByDriverSpec : BaseSpecification<SOSLog>
{
    public SOSByDriverSpec(Guid driverId, PaginationDto pagination)
    {
        SetCriteria(s => s.DriverId == driverId);
        SetOrderByDescending(s => s.ActivatedAt);
        ApplyPaging((pagination.Page - 1) * pagination.PageSize, pagination.PageSize);
    }
}

internal class SOSByDriverCountSpec : BaseSpecification<SOSLog>
{
    public SOSByDriverCountSpec(Guid driverId)
    {
        SetCriteria(s => s.DriverId == driverId);
    }
}
