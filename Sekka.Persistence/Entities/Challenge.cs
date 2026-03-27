using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Challenge : BaseEntity<Guid>
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ChallengeType ChallengeType { get; set; }
    public string TargetMetric { get; set; } = null!;
    public decimal TargetValue { get; set; }
    public int RewardPoints { get; set; }
    public string? BadgeName { get; set; }
    public string? BadgeIconUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<DriverAchievement> DriverAchievements { get; set; } = new List<DriverAchievement>();
}
