namespace Sekka.Core.DTOs.HealthScore;

public class AccountHealthDto
{
    public int OverallScore { get; set; }
    public int SuccessRateScore { get; set; }
    public int CustomerRatingScore { get; set; }
    public int CommitmentScore { get; set; }
    public int ActivityScore { get; set; }
    public int CashHandlingScore { get; set; }
    public string Status { get; set; } = null!;
    public DateTime LastCalculatedAt { get; set; }
    public string Trend { get; set; } = null!;
}

public class HealthTipDto
{
    public string Category { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal CurrentValue { get; set; }
    public decimal TargetValue { get; set; }
    public int ImpactOnScore { get; set; }
    public int Priority { get; set; }
}
