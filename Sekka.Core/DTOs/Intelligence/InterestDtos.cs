namespace Sekka.Core.DTOs.Intelligence;

public class CustomerInterestProfileDto
{
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string CustomerPhone { get; set; } = null!;
    public string EngagementLevel { get; set; } = null!;
    public decimal LifetimeValue { get; set; }
    public int TotalOrders { get; set; }
    public List<CustomerInterestDto> TopInterests { get; set; } = new();
    public List<SegmentBriefDto> CurrentSegments { get; set; } = new();
    public CustomerBehaviorSummaryDto? BehaviorSummary { get; set; }
    public DateTime? LastOrderDate { get; set; }
    public int? DaysSinceLastOrder { get; set; }
    public RfmScoreDto? RfmScore { get; set; }
}

public class CustomerInterestDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string CategoryNameAr { get; set; } = null!;
    public string? CategoryColor { get; set; }
    public decimal Score { get; set; }
    public int SignalCount { get; set; }
    public string TrendDirection { get; set; } = null!;
    public decimal ConfidenceLevel { get; set; }
    public DateTime? LastSignalAt { get; set; }
}

public class CustomerBehaviorSummaryDto
{
    public string? PreferredOrderTime { get; set; }
    public string? PreferredDayOfWeek { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal OrderFrequencyPerMonth { get; set; }
    public string? PreferredPaymentMethod { get; set; }
    public List<string> PreferredAreas { get; set; } = new();
    public string SpendingTier { get; set; } = null!;
    public List<BehaviorPatternDto> Patterns { get; set; } = new();
}

public class BehaviorPatternDto
{
    public string PatternType { get; set; } = null!;
    public string PatternKey { get; set; } = null!;
    public string PatternValue { get; set; } = null!;
    public decimal Confidence { get; set; }
    public int Occurrences { get; set; }
}

public class CustomerRecommendationDto
{
    public Guid Id { get; set; }
    public string RecommendationType { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? CategoryName { get; set; }
    public string? CategoryColor { get; set; }
    public decimal RelevanceScore { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RfmScoreDto
{
    public int RecencyScore { get; set; }
    public int FrequencyScore { get; set; }
    public int MonetaryScore { get; set; }
    public int TotalScore { get; set; }
    public string Segment { get; set; } = null!;
}

public class SegmentBriefDto
{
    public Guid SegmentId { get; set; }
    public string Name { get; set; } = null!;
    public string NameAr { get; set; } = null!;
    public string? ColorHex { get; set; }
    public DateTime JoinedAt { get; set; }
}

public class TopInterestsQueryDto
{
    public int Limit { get; set; } = 10;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class InterestCategorySummaryDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string CategoryNameAr { get; set; } = null!;
    public string? CategoryColor { get; set; }
    public int CustomerCount { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageScore { get; set; }
    public string TrendDirection { get; set; } = null!;
}

public class CustomerSegmentSummaryDto
{
    public Guid SegmentId { get; set; }
    public string Name { get; set; } = null!;
    public string NameAr { get; set; } = null!;
    public string? ColorHex { get; set; }
    public int MemberCount { get; set; }
    public decimal PercentageOfTotal { get; set; }
}
