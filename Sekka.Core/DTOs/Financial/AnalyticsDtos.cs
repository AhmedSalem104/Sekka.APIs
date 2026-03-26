namespace Sekka.Core.DTOs.Financial;

public class SourceBreakdownDto
{
    public string Source { get; set; } = null!;
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
    public double Percentage { get; set; }
}

public class CustomerProfitabilityDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public int OrderCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public double SuccessRate { get; set; }
}

public class RegionAnalysisDto
{
    public string Region { get; set; } = null!;
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
    public double AverageDeliveryTimeMinutes { get; set; }
    public double SuccessRate { get; set; }
}

public class TimeAnalysisDto
{
    public int Hour { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
    public double AverageDeliveryTimeMinutes { get; set; }
}

public class CancellationReportDto
{
    public string Reason { get; set; } = null!;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public decimal LostRevenue { get; set; }
}

public class ProfitabilityTrendDto
{
    public string Period { get; set; } = null!;
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetProfit { get; set; }
    public double ProfitMargin { get; set; }
}
