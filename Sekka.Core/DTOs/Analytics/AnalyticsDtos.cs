namespace Sekka.Core.DTOs.Analytics;

public class SourceBreakdownDto
{
    public List<SourceEntryDto> Sources { get; set; } = new();
    public string TopSource { get; set; } = null!;
    public decimal TopSourcePercentage { get; set; }
}

public class SourceEntryDto
{
    public string SourceType { get; set; } = null!;
    public string SourceName { get; set; } = null!;
    public int OrderCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageProfit { get; set; }
    public decimal Percentage { get; set; }
}

public class CustomerProfitabilityDto
{
    public List<CustomerProfitEntryDto> TopProfitable { get; set; } = new();
    public List<CustomerProfitEntryDto> LeastProfitable { get; set; } = new();
    public decimal AverageCustomerValue { get; set; }
}

public class CustomerProfitEntryDto
{
    public string CustomerName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AvgOrderValue { get; set; }
    public decimal FailureRate { get; set; }
    public decimal ProfitScore { get; set; }
}

public class RegionAnalysisDto
{
    public List<RegionPerformanceDto> Regions { get; set; } = new();
    public string BestRegion { get; set; } = null!;
    public string WorstRegion { get; set; } = null!;
}

public class RegionPerformanceDto
{
    public string RegionName { get; set; } = null!;
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
    public double AvgDeliveryTime { get; set; }
    public decimal SuccessRate { get; set; }
}

public class TimeAnalysisDto
{
    public List<HourlyBreakdownDto> HourlyBreakdown { get; set; } = new();
    public int BestHour { get; set; }
    public string BestDay { get; set; } = null!;
    public List<int> PeakHours { get; set; } = new();
}

public class HourlyBreakdownDto
{
    public int Hour { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}

public class CancellationReportDto
{
    public int TotalCancellations { get; set; }
    public decimal TotalLossAmount { get; set; }
    public List<CancellationReasonEntryDto> TopReasons { get; set; } = new();
    public List<CustomerProfitEntryDto> TopCancellingCustomers { get; set; } = new();
    public decimal CancellationRate { get; set; }
}

public class CancellationReasonEntryDto
{
    public string Reason { get; set; } = null!;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
    public decimal TotalLoss { get; set; }
}

public class ProfitabilityTrendsDto
{
    public string Period { get; set; } = null!;
    public List<TrendPointDto> DataPoints { get; set; } = new();
    public decimal AvgProfit { get; set; }
    public string TrendDirection { get; set; } = null!;
}

public class TrendPointDto
{
    public DateOnly Date { get; set; }
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetProfit { get; set; }
    public int OrderCount { get; set; }
}
