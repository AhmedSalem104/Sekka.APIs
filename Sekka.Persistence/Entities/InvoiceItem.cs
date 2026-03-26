using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class InvoiceItem : BaseEntity<Guid>
{
    public Guid InvoiceId { get; set; }
    public Guid? OrderId { get; set; }
    public string Description { get; set; } = null!;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    // Navigation
    public Invoice Invoice { get; set; } = null!;
    public Order? Order { get; set; }
}
