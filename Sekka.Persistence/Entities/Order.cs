using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Order : SoftDeletableEntity<Guid>
{
    public Guid DriverId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? PartnerId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public decimal CommissionAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public OrderStatus Status { get; set; }
    public OrderPriority Priority { get; set; }
    public OrderSourceType SourceType { get; set; }
    public string? PickupAddress { get; set; }
    public double? PickupLatitude { get; set; }
    public double? PickupLongitude { get; set; }
    public string DeliveryAddress { get; set; } = null!;
    public double? DeliveryLatitude { get; set; }
    public double? DeliveryLongitude { get; set; }
    public double? DistanceKm { get; set; }
    public string? Notes { get; set; }
    public int ItemCount { get; set; } = 1;
    public DateTime? TimeWindowStart { get; set; }
    public DateTime? TimeWindowEnd { get; set; }
    public DateOnly? ScheduledDate { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }
    public int? SequenceIndex { get; set; }
    public decimal? ExpectedChangeAmount { get; set; }
    public decimal? ActualCollectedAmount { get; set; }
    public string? ReturnReason { get; set; }
    public string? PartialDeliveryNote { get; set; }
    public string? IdempotencyKey { get; set; }
    public decimal? WorthScore { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? ArrivedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? FailedAt { get; set; }

    // Navigation properties
    public Driver Driver { get; set; } = null!;
    public ICollection<DeliveryAttempt> DeliveryAttempts { get; set; } = new List<DeliveryAttempt>();
    public ICollection<OrderPhoto> Photos { get; set; } = new List<OrderPhoto>();
    public ICollection<AddressSwapLog> AddressSwapLogs { get; set; } = new List<AddressSwapLog>();
    public CancellationLog? CancellationLog { get; set; }
    public OrderSourceTag? SourceTag { get; set; }
    public ICollection<WaitingTimer> WaitingTimers { get; set; } = new List<WaitingTimer>();
    public ICollection<OrderTransferLog> TransferLogs { get; set; } = new List<OrderTransferLog>();
    public ICollection<VoiceMemo> VoiceMemos { get; set; } = new List<VoiceMemo>();
    public TrackingLink? TrackingLink { get; set; }
}
