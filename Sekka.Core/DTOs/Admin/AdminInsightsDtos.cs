namespace Sekka.Core.DTOs.Admin;

public class InsightsOverviewDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int AtRiskCustomers { get; set; }
    public int ChurnedCustomers { get; set; }
    public decimal AverageEngagementScore { get; set; }
    public decimal AverageLifetimeValue { get; set; }
    public int TotalSegments { get; set; }
    public int TotalCategories { get; set; }
    public int TotalSignalsToday { get; set; }
    public int TotalRecommendationsGenerated { get; set; }
}

public class InterestHeatmapDto
{
    public List<HeatmapCellDto> Cells { get; set; } = new();
}

public class HeatmapCellDto
{
    public string CategoryName { get; set; } = null!;
    public string SegmentName { get; set; } = null!;
    public decimal Score { get; set; }
    public int CustomerCount { get; set; }
}

public class InterestTrendDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string CategoryNameAr { get; set; } = null!;
    public string? CategoryColor { get; set; }
    public string TrendDirection { get; set; } = null!;
    public decimal ChangePercent { get; set; }
    public List<TrendDataPointDto> DataPoints { get; set; } = new();
}

public class TrendDataPointDto
{
    public DateOnly Date { get; set; }
    public decimal Value { get; set; }
    public int Count { get; set; }
}

public class TrendsQueryDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string Period { get; set; } = "daily";
    public int? Limit { get; set; }
}

public class EngagementDistributionDto
{
    public int HighEngagement { get; set; }
    public int MediumEngagement { get; set; }
    public int LowEngagement { get; set; }
    public int AtRisk { get; set; }
    public int Churned { get; set; }
    public decimal HighPercentage { get; set; }
    public decimal MediumPercentage { get; set; }
    public decimal LowPercentage { get; set; }
    public decimal AtRiskPercentage { get; set; }
    public decimal ChurnedPercentage { get; set; }
}

public class RfmAnalysisDto
{
    public int TotalCustomersAnalyzed { get; set; }
    public decimal AverageRecencyScore { get; set; }
    public decimal AverageFrequencyScore { get; set; }
    public decimal AverageMonetaryScore { get; set; }
    public Dictionary<string, int> SegmentDistribution { get; set; } = new();
    public List<RfmSegmentDetailDto> Segments { get; set; } = new();
}

public class RfmSegmentDetailDto
{
    public string SegmentName { get; set; } = null!;
    public int CustomerCount { get; set; }
    public decimal AverageRecency { get; set; }
    public decimal AverageFrequency { get; set; }
    public decimal AverageMonetary { get; set; }
    public decimal Percentage { get; set; }
}

public class GlobalBehaviorSummaryDto
{
    public string MostPopularOrderTime { get; set; } = null!;
    public string MostPopularDayOfWeek { get; set; } = null!;
    public decimal AverageOrderValue { get; set; }
    public decimal AverageOrderFrequency { get; set; }
    public string MostPopularPaymentMethod { get; set; } = null!;
    public List<string> TopAreas { get; set; } = new();
    public Dictionary<string, decimal> SpendingTierDistribution { get; set; } = new();
    public Dictionary<string, int> PaymentMethodDistribution { get; set; } = new();
}

public class CategoryPerformanceDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string CategoryNameAr { get; set; } = null!;
    public string? CategoryColor { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int UniqueCustomers { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal GrowthPercent { get; set; }
    public string TrendDirection { get; set; } = null!;
}
