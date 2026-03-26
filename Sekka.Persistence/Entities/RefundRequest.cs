using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class RefundRequest : AuditableEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid DriverId { get; set; }
    public decimal Amount { get; set; }
    public RefundReason RefundReason { get; set; }
    public RefundStatus Status { get; set; }
    public string? Description { get; set; }
    public string? AdminNotes { get; set; }
    public string? ProcessedBy { get; set; }
    public DateTime? ProcessedAt { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
}
