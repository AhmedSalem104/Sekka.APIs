using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Invoice : BaseEntity<Guid>
{
    public string InvoiceNumber { get; set; } = null!;
    public Guid DriverId { get; set; }
    public Guid? PartnerId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? PdfUrl { get; set; }
    public string? Notes { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public Partner? Partner { get; set; }
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}
