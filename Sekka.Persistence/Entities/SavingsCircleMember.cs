using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class SavingsCircleMember : BaseEntity<Guid>
{
    public Guid CircleId { get; set; }
    public Guid DriverId { get; set; }
    public int TurnOrder { get; set; }
    public CircleMemberStatus Status { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public SavingsCircle Circle { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
}
