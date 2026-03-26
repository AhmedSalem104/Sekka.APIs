using Sekka.Core.DTOs.Common;

namespace Sekka.Core.DTOs.Financial;

public class DailyStatsDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public DateOnly Date { get; set; }
    public int TotalOrders { get; set; }
    public int SuccessfulOrders { get; set; }
    public int FailedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public int PostponedOrders { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalCommissions { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public double TotalDistanceKm { get; set; }
    public int TimeWorkedMinutes { get; set; }
    public decimal CashCollected { get; set; }
    public int? AverageDeliveryTimeMinutes { get; set; }
    public string? BestRegion { get; set; }
    public string? BestTimeSlot { get; set; }
}

public class WeeklyStatsDto
{
    public DateOnly WeekStart { get; set; }
    public DateOnly WeekEnd { get; set; }
    public int TotalOrders { get; set; }
    public int SuccessfulOrders { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal NetProfit { get; set; }
    public double SuccessRate { get; set; }
    public List<DailyStatsDto> DailyBreakdown { get; set; } = new();
}

public class MonthlyStatsDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalOrders { get; set; }
    public int SuccessfulOrders { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal NetProfit { get; set; }
    public double SuccessRate { get; set; }
    public List<WeeklyStatsDto> WeeklyBreakdown { get; set; } = new();
}

public class HeatmapDataDto
{
    public DateOnly Date { get; set; }
    public int Hour { get; set; }
    public int OrderCount { get; set; }
    public decimal Earnings { get; set; }
}

public class StatsFilterDto : PaginationDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class AdminStatsFilterDto : PaginationDto
{
    public Guid? DriverId { get; set; }
    public Guid? RegionId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class PlatformStatsDto
{
    public int TotalDrivers { get; set; }
    public int ActiveDrivers { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCommissions { get; set; }
    public double AverageSuccessRate { get; set; }
}

public class DriverRankingDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public int TotalOrders { get; set; }
    public decimal TotalEarnings { get; set; }
    public double SuccessRate { get; set; }
    public int Rank { get; set; }
}
