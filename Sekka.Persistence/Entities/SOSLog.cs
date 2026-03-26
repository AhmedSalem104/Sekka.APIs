using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class SOSLog : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public SOSStatus Status { get; set; }
    public string? NotifiedContacts { get; set; }
    public bool LocationSharedWithFamily { get; set; } = true;
    public bool AdminNotified { get; set; }
    public string? Notes { get; set; }
    public DateTime ActivatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public SOSEscalationLevel EscalationLevel { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string? AcknowledgedBy { get; set; }
    public string? ResolvedBy { get; set; }

    public Driver Driver { get; set; } = null!;
}
