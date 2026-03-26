using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class OrderDispute : AuditableEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid DriverId { get; set; }
    public DisputeType DisputeType { get; set; }
    public DisputeStatus Status { get; set; }
    public string Description { get; set; } = null!;
    public string? EvidenceUrls { get; set; }
    public string? AdminNotes { get; set; }
    public string? Resolution { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
}
