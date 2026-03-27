using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Referral : BaseEntity<Guid>
{
    public Guid ReferrerDriverId { get; set; }
    public Guid? ReferredDriverId { get; set; }
    public string ReferralCode { get; set; } = null!;
    public string? ReferredPhone { get; set; }
    public ReferralStatus Status { get; set; }
    public RewardType RewardType { get; set; }
    public bool RewardGiven { get; set; }
    public bool ReferrerRewardGiven { get; set; }
    public DateTime? RegisteredAt { get; set; }
    public DateTime? RewardedAt { get; set; }

    // Navigation
    public Driver ReferrerDriver { get; set; } = null!;
    public Driver? ReferredDriver { get; set; }
}
