using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Partner : AuditableEntity<Guid>
{
    public Guid DriverId { get; set; }
    public string Name { get; set; } = null!;
    public PartnerType PartnerType { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public CommissionType CommissionType { get; set; }
    public decimal CommissionValue { get; set; }
    public PaymentMethod DefaultPaymentMethod { get; set; }
    public string Color { get; set; } = "#3B82F6";
    public string? LogoUrl { get; set; }
    public string? ReceiptHeader { get; set; }
    public bool IsActive { get; set; } = true;

    // Verification
    public VerificationStatus VerificationStatus { get; set; }
    public string? VerificationDocumentUrl { get; set; }
    public string? VerificationNote { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public Guid? VerifiedByAdminId { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public Driver? VerifiedByAdmin { get; set; }
    public ICollection<PickupPoint> PickupPoints { get; set; } = new List<PickupPoint>();
    public ICollection<CallerIdNote> CallerIdNotes { get; set; } = new List<CallerIdNote>();
}
