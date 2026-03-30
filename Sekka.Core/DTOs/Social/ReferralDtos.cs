using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Social;

public class ReferralCodeDto
{
    public string ReferralCode { get; set; } = null!;
    public string ShareUrl { get; set; } = null!;
    public string ShareMessage { get; set; } = null!;
    public int TotalReferred { get; set; }
    public decimal TotalRewards { get; set; }
}

public class ReferralStatsDto
{
    public string ReferralCode { get; set; } = null!;
    public string ShareUrl { get; set; } = null!;
    public int TotalReferrals { get; set; }
    public int PendingReferrals { get; set; }
    public int CompletedReferrals { get; set; }
    public int RewardedReferrals { get; set; }
    public int TotalPointsEarned { get; set; }
    public decimal TotalCashEarned { get; set; }
    public List<ReferralDto> RecentReferrals { get; set; } = new();
}

public class ApplyReferralCodeDto
{
    public string ReferralCode { get; set; } = null!;
}

public class ReferralDto
{
    public Guid Id { get; set; }
    public string? ReferredDriverName { get; set; }
    public string? ReferredPhone { get; set; }
    public ReferralStatus Status { get; set; }
    public string StatusText { get; set; } = null!;
    public RewardType RewardType { get; set; }
    public bool RewardGiven { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RegisteredAt { get; set; }
    public DateTime? RewardedAt { get; set; }
}
