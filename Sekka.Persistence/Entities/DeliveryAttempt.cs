using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class DeliveryAttempt : BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public int AttemptNumber { get; set; }
    public DeliveryFailReason Status { get; set; }
    public string? Reason { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool AutoMessageSent { get; set; }
    public string? Notes { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation
    public Order Order { get; set; } = null!;
}
