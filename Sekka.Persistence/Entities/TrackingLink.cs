using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class TrackingLink : BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public string TrackingCode { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public int ViewCount { get; set; }
    public DateTime ExpiresAt { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
}
