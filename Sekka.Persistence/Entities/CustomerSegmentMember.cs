using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class CustomerSegmentMember : BaseEntity<Guid>
{
    public Guid SegmentId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid DriverId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public decimal? Score { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ExitedAt { get; set; }

    // Navigation
    public CustomerSegment Segment { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
}
