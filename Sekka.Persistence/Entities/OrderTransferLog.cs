using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class OrderTransferLog : BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid FromDriverId { get; set; }
    public Guid? ToDriverId { get; set; }
    public string? TransferReason { get; set; }
    public string? DeepLinkToken { get; set; }
    public TransferStatus Status { get; set; }
    public OrderStatus? PreviousOrderStatus { get; set; }
    public DateTime TransferredAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedAt { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public Driver FromDriver { get; set; } = null!;
    public Driver? ToDriver { get; set; }
}
