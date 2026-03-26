using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Order;

public class AdminOrderFilterDto : PaginationDto
{
    public OrderStatus? Status { get; set; }
    public Guid? DriverId { get; set; }
    public Guid? PartnerId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public string? SearchTerm { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public OrderPriority? Priority { get; set; }
}

public class AdminOrderDto : OrderDto
{
    public string DriverName { get; set; } = null!;
    public string DriverPhone { get; set; } = null!;
}

public class OrderBoardDto
{
    public List<OrderListDto> Pending { get; set; } = new();
    public List<OrderListDto> Accepted { get; set; } = new();
    public List<OrderListDto> PickedUp { get; set; } = new();
    public List<OrderListDto> InTransit { get; set; } = new();
    public List<OrderListDto> Delivered { get; set; } = new();
    public List<OrderListDto> Failed { get; set; } = new();
    public List<OrderListDto> Cancelled { get; set; } = new();
}

public class AssignOrderDto
{
    public Guid DriverId { get; set; }
}

public class AdminCreateOrderDto : CreateOrderDto
{
    public Guid DriverId { get; set; }
}

public class AutoDistributeDto
{
    public List<Guid> OrderIds { get; set; } = new();
    public AssignmentStrategy Strategy { get; set; } = AssignmentStrategy.Balanced;
}

public class DistributionResultDto
{
    public int TotalOrders { get; set; }
    public int AssignedOrders { get; set; }
    public int UnassignedOrders { get; set; }
    public List<AssignmentResultItemDto> Assignments { get; set; } = new();
}

public class AssignmentResultItemDto
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    public bool IsAssigned { get; set; }
    public string? FailureReason { get; set; }
}

public class OverrideStatusDto
{
    public OrderStatus NewStatus { get; set; }
    public string Reason { get; set; } = null!;
}

public class ExportFilterDto : AdminOrderFilterDto
{
    public string Format { get; set; } = "csv";
}

public class OrderTimelineDto
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public List<OrderTimelineEventDto> Events { get; set; } = new();
}

public class OrderTimelineEventDto
{
    public string Event { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string? Actor { get; set; }
}
