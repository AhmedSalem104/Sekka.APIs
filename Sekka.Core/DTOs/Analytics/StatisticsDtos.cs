using Sekka.Core.DTOs.Common;

namespace Sekka.Core.DTOs.Analytics;

public class DailyStatsDto
{
    public DateOnly Date { get; set; }
    public int TotalOrders { get; set; }
    public int Successful { get; set; }
    public int Failed { get; set; }
    public int Cancelled { get; set; }
    public decimal Earnings { get; set; }
    public decimal Commissions { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetProfit { get; set; }
    public double DistanceKm { get; set; }
    public TimeSpan TimeWorked { get; set; }
    public decimal Cash { get; set; }
    public double? AvgDeliveryTime { get; set; }
    public string? BestRegion { get; set; }
    public string? BestTimeSlot { get; set; }
    public decimal SuccessRate { get; set; }
}

public class WeeklyStatsDto
{
    public DateOnly WeekStart { get; set; }
    public DateOnly WeekEnd { get; set; }
    public List<DailyStatsDto> DailyBreakdown { get; set; } = new();
    public int TotalOrders { get; set; }
    public decimal TotalEarnings { get; set; }
    public string BestDay { get; set; } = null!;
    public string WorstDay { get; set; } = null!;
    public double AvgOrdersPerDay { get; set; }
    public decimal ComparedToLastWeek { get; set; }
}

public class MonthlyStatsDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public List<WeeklyStatsDto> WeeklyBreakdown { get; set; } = new();
    public int TotalOrders { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public decimal ComparedToLastMonth { get; set; }
    public List<string> TopPartners { get; set; } = new();
    public List<string> TopRegions { get; set; } = new();
}

public class HeatmapDto
{
    public List<HeatmapPointDto> Points { get; set; } = new();
    public string BestArea { get; set; } = null!;
    public string BestTimeSlot { get; set; } = null!;
    public decimal TotalEarningsInPeriod { get; set; }
}

public class HeatmapPointDto
{
    public double Lat { get; set; }
    public double Lng { get; set; }
    public int OrderCount { get; set; }
    public decimal Earnings { get; set; }
    public double Intensity { get; set; }
}

public class ExportFilterDto
{
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public string Format { get; set; } = "pdf";
    public bool IncludeOrders { get; set; }
    public bool IncludeExpenses { get; set; }
    public bool IncludeSettlements { get; set; }
}
