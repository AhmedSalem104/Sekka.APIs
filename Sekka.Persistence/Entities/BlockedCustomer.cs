using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class BlockedCustomer : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public string CustomerPhone { get; set; } = null!;
    public Guid? CustomerId { get; set; }
    public string? Reason { get; set; }
    public bool IsCommunityReport { get; set; }
    public DateTime BlockedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Driver Driver { get; set; } = null!;
    public Customer? Customer { get; set; }
}
