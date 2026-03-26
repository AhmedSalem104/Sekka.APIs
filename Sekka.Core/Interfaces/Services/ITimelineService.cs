using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Enums;

namespace Sekka.Core.Interfaces.Services;

public interface ITimelineService
{
    Task<Result<DailyTimelineDto>> GetDailyAsync(Guid driverId, DateOnly date);
    Task<Result<List<DailyTimelineDto>>> GetRangeAsync(Guid driverId, DateOnly dateFrom, DateOnly dateTo);
    Task<Result<DailyTimelineDto>> GetFilteredAsync(Guid driverId, DateOnly date, List<TimelineEventType> eventTypes);
}
