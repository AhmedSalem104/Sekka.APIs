using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class TimelineService : ITimelineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TimelineService> _logger;

    public TimelineService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TimelineService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<DailyTimelineDto>> GetDailyAsync(Guid driverId, DateOnly date)
    {
        _logger.LogInformation("Daily timeline requested by driver {DriverId} for date {Date}", driverId, date);
        return await Task.FromResult(Result<DailyTimelineDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الجدول الزمني")));
    }

    public async Task<Result<List<DailyTimelineDto>>> GetRangeAsync(Guid driverId, DateOnly dateFrom, DateOnly dateTo)
    {
        _logger.LogInformation("Timeline range requested by driver {DriverId} from {DateFrom} to {DateTo}", driverId, dateFrom, dateTo);
        return await Task.FromResult(Result<List<DailyTimelineDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الجدول الزمني")));
    }

    public async Task<Result<DailyTimelineDto>> GetFilteredAsync(Guid driverId, DateOnly date, List<TimelineEventType> eventTypes)
    {
        _logger.LogInformation("Filtered timeline requested by driver {DriverId} for date {Date}", driverId, date);
        return await Task.FromResult(Result<DailyTimelineDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الجدول الزمني")));
    }
}
