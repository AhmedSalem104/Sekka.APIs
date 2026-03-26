using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class SyncQueue : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public SyncOperation OperationType { get; set; }
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public SyncStatus Status { get; set; }
    public string? ConflictResolution { get; set; }
    public DateTime? SyncedAt { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
}
