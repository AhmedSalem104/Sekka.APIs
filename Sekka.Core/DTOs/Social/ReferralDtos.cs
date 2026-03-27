using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Social;

public class ReferralCodeDto
{
    public string ReferralCode { get; set; } = null!;
    public string ShareUrl { get; set; } = null!;
}

public class ReferralStatsDto
{
    public string ReferralCode { get; set; } = null!;
    public int TotalReferrals { get; set; }
    public int CompletedReferrals { get; set; }
    public int PendingReferrals { get; set; }
    public int TotalPointsEarned { get; set; }
}

public class ApplyReferralCodeDto
{
    public string ReferralCode { get; set; } = null!;
}

public class ReferralDto
{
    public Guid Id { get; set; }
    public Guid ReferrerDriverId { get; set; }
    public Guid? ReferredDriverId { get; set; }
    public string ReferralCode { get; set; } = null!;
    public string? ReferredPhone { get; set; }
    public ReferralStatus Status { get; set; }
    public RewardType RewardType { get; set; }
    public bool RewardGiven { get; set; }
    public DateTime? RegisteredAt { get; set; }
    public DateTime? RewardedAt { get; set; }
}
