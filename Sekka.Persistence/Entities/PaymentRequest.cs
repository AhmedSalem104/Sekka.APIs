using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class PaymentRequest : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public PaymentPurpose PaymentPurpose { get; set; }
    public decimal Amount { get; set; }
    public ManualPaymentMethod PaymentMethod { get; set; }
    public string ReferenceCode { get; set; } = null!;
    public string? ProofImageUrl { get; set; }
    public string? SenderPhone { get; set; }
    public string? SenderName { get; set; }
    public string? Notes { get; set; }
    public PaymentRequestStatus Status { get; set; }
    public Guid? AdminId { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public DateTime? ExpiresAt { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public Driver? Admin { get; set; }
}
