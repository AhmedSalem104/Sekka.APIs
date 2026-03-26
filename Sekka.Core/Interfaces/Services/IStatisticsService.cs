using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;

namespace Sekka.Core.Interfaces.Services;

public interface IStatisticsService
{
    Task<Result<DailyStatsDto>> GetDailyAsync(Guid driverId, DateOnly date);
    Task<Result<WeeklyStatsDto>> GetWeeklyAsync(Guid driverId, DateOnly weekStart);
    Task<Result<MonthlyStatsDto>> GetMonthlyAsync(Guid driverId, int month, int year);
    Task<Result<List<HeatmapDataDto>>> GetHeatmapAsync(Guid driverId, DateTime dateFrom, DateTime dateTo);
}
