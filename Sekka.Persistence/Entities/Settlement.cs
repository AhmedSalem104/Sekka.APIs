using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Settlement : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public Guid PartnerId { get; set; }
    public decimal Amount { get; set; }
    public SettlementType SettlementType { get; set; }
    public int OrderCount { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptImageUrl { get; set; }
    public bool WhatsAppSent { get; set; }
    public DateTime SettledAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Driver Driver { get; set; } = null!;
    public Partner Partner { get; set; } = null!;
    public ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
}
