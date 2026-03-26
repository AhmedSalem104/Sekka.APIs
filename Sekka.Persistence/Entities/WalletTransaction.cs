using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class WalletTransaction : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? SettlementId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Description { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public Order? Order { get; set; }
    public Settlement? Settlement { get; set; }
}
