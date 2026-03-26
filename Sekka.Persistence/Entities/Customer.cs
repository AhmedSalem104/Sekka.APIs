using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Customer : AuditableEntity<Guid>
{
    public Guid DriverId { get; set; }
    public string Phone { get; set; } = null!;
    public string? Name { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalDeliveries { get; set; }
    public int SuccessfulDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockReason { get; set; }
    public string? Notes { get; set; }
    public PaymentMethod? PreferredPaymentMethod { get; set; }
    public DateTime? LastDeliveryDate { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public ICollection<CallerIdNote> CallerIdNotes { get; set; } = new List<CallerIdNote>();
    public ICollection<BlockedCustomer> BlockedEntries { get; set; } = new List<BlockedCustomer>();
    public ICollection<VoiceMemo> VoiceMemos { get; set; } = new List<VoiceMemo>();
}
