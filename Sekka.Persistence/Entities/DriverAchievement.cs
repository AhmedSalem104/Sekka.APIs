using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class DriverAchievement : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public Guid ChallengeId { get; set; }
    public decimal CurrentProgress { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int PointsEarned { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public Challenge Challenge { get; set; } = null!;
}
