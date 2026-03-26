using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Order;

public class CreateOrderDto
{
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public Guid? PartnerId { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public OrderPriority Priority { get; set; }
    public string? PickupAddress { get; set; }
    public double? PickupLatitude { get; set; }
    public double? PickupLongitude { get; set; }
    public string DeliveryAddress { get; set; } = null!;
    public double? DeliveryLatitude { get; set; }
    public double? DeliveryLongitude { get; set; }
    public string? Notes { get; set; }
    public int ItemCount { get; set; } = 1;
    public DateTime? TimeWindowStart { get; set; }
    public DateTime? TimeWindowEnd { get; set; }
    public DateOnly? ScheduledDate { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }
    public decimal? ExpectedChangeAmount { get; set; }
    public string? IdempotencyKey { get; set; }
}

public class UpdateOrderDto
{
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public OrderPriority? Priority { get; set; }
    public string? DeliveryAddress { get; set; }
    public double? DeliveryLatitude { get; set; }
    public double? DeliveryLongitude { get; set; }
    public string? Notes { get; set; }
    public int? ItemCount { get; set; }
    public DateTime? TimeWindowStart { get; set; }
    public DateTime? TimeWindowEnd { get; set; }
}

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? PartnerName { get; set; }
    public string? PartnerColor { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public decimal CommissionAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public OrderStatus Status { get; set; }
    public OrderPriority Priority { get; set; }
    public string DeliveryAddress { get; set; } = null!;
    public double? DistanceKm { get; set; }
    public int? SequenceIndex { get; set; }
    public decimal? WorthScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}

public class OrderListDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public string? CustomerName { get; set; }
    public decimal Amount { get; set; }
    public OrderStatus Status { get; set; }
    public OrderPriority Priority { get; set; }
    public string DeliveryAddress { get; set; } = null!;
    public string? PartnerColor { get; set; }
    public int? SequenceIndex { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderDetailDto : OrderDto
{
    public string? PickupAddress { get; set; }
    public string? Notes { get; set; }
    public int ItemCount { get; set; }
    public DateTime? TimeWindowStart { get; set; }
    public DateTime? TimeWindowEnd { get; set; }
    public bool IsRecurring { get; set; }
    public decimal? ExpectedChangeAmount { get; set; }
    public decimal? ActualCollectedAmount { get; set; }
    public OrderSourceType SourceType { get; set; }
    public List<OrderPhotoDto> Photos { get; set; } = new();
    public List<DeliveryAttemptDto> DeliveryAttempts { get; set; } = new();
    public List<AddressSwapLogDto> AddressSwapLogs { get; set; } = new();
    public List<WaitingTimerDto> WaitingTimers { get; set; } = new();
    public CancellationLogDto? CancellationLog { get; set; }
    public OrderSourceTagDto? SourceTag { get; set; }
}

public class OrderFilterDto : PaginationDto
{
    public OrderStatus? Status { get; set; }
    public Guid? PartnerId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public string? SearchTerm { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public OrderPriority? Priority { get; set; }
}
