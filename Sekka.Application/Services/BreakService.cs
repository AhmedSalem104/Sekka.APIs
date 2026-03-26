using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Vehicle;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class BreakService : IBreakService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BreakService> _logger;

    public BreakService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BreakService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<BreakLogDto>> StartBreakAsync(Guid driverId, StartBreakDto dto)
    {
        var repo = _unitOfWork.GetRepository<BreakLog, Guid>();

        // Check if there's an active break (no EndTime)
        var activeSpec = new ActiveBreakSpec(driverId);
        var activeBreaks = await repo.ListAsync(activeSpec);
        if (activeBreaks.Any())
            return Result<BreakLogDto>.Conflict("يوجد استراحة نشطة بالفعل. قم بإنهائها أولاً");

        var breakLog = new BreakLog
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            StartTime = DateTime.UtcNow,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            LocationDescription = dto.LocationDescription,
            EnergyBefore = dto.EnergyBefore
        };

        await repo.AddAsync(breakLog);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Break started for driver {DriverId}, energy before: {Energy}",
            driverId, dto.EnergyBefore);

        return Result<BreakLogDto>.Success(_mapper.Map<BreakLogDto>(breakLog));
    }

    public async Task<Result<BreakLogDto>> EndBreakAsync(Guid driverId, EndBreakDto dto)
    {
        var repo = _unitOfWork.GetRepository<BreakLog, Guid>();

        var activeSpec = new ActiveBreakSpec(driverId);
        var activeBreaks = await repo.ListAsync(activeSpec);
        var breakLog = activeBreaks.FirstOrDefault();

        if (breakLog == null)
            return Result<BreakLogDto>.NotFound("لا توجد استراحة نشطة لإنهائها");

        breakLog.EndTime = DateTime.UtcNow;
        breakLog.DurationMinutes = (int)(breakLog.EndTime.Value - breakLog.StartTime).TotalMinutes;
        breakLog.EnergyAfter = dto.EnergyAfter;

        repo.Update(breakLog);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Break ended for driver {DriverId}, duration: {Duration} min, energy after: {Energy}",
            driverId, breakLog.DurationMinutes, dto.EnergyAfter);

        return Result<BreakLogDto>.Success(_mapper.Map<BreakLogDto>(breakLog));
    }

    public Task<Result<BreakSuggestionDto>> GetSuggestionAsync(Guid driverId)
    {
        // TODO: Implement smart break suggestion based on:
        // - Time since last break
        // - Number of deliveries completed
        // - Driver's energy level trend
        // - Time of day
        // For now, return a default suggestion
        var suggestion = new BreakSuggestionDto
        {
            ShouldBreak = false,
            Urgency = BreakUrgency.Low,
            SuggestedDurationMinutes = 15,
            Reason = "لا توجد بيانات كافية لتقديم اقتراح. استمر في العمل وسنقترح استراحة عند الحاجة",
            NearbySpots = new List<ParkingSpotDto>()
        };

        return Task.FromResult(Result<BreakSuggestionDto>.Success(suggestion));
    }

    public async Task<Result<PagedResult<BreakLogDto>>> GetHistoryAsync(Guid driverId, PaginationDto pagination)
    {
        var repo = _unitOfWork.GetRepository<BreakLog, Guid>();

        var spec = new BreakHistorySpec(driverId, pagination);
        var items = await repo.ListAsync(spec);

        var countSpec = new BreakHistoryCountSpec(driverId);
        var total = await repo.CountAsync(countSpec);

        var dtos = _mapper.Map<List<BreakLogDto>>(items);
        return Result<PagedResult<BreakLogDto>>.Success(
            new PagedResult<BreakLogDto>(dtos, total, pagination.Page, pagination.PageSize));
    }
}

// ── Specifications ──

internal class ActiveBreakSpec : BaseSpecification<BreakLog>
{
    public ActiveBreakSpec(Guid driverId)
    {
        SetCriteria(b => b.DriverId == driverId && b.EndTime == null);
        SetOrderByDescending(b => b.StartTime);
    }
}

internal class BreakHistorySpec : BaseSpecification<BreakLog>
{
    public BreakHistorySpec(Guid driverId, PaginationDto pagination)
    {
        SetCriteria(b => b.DriverId == driverId);
        SetOrderByDescending(b => b.StartTime);
        ApplyPaging((pagination.Page - 1) * pagination.PageSize, pagination.PageSize);
    }
}

internal class BreakHistoryCountSpec : BaseSpecification<BreakLog>
{
    public BreakHistoryCountSpec(Guid driverId)
    {
        SetCriteria(b => b.DriverId == driverId);
    }
}
