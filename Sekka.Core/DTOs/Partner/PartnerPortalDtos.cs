using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Partner;

public class PartnerDashboardDto
{
    public int TotalOrders { get; set; }
    public int ActiveOrders { get; set; }
    public int TodayOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal PendingSettlement { get; set; }
    public decimal SuccessRate { get; set; }
    public List<Order.OrderListDto> RecentOrders { get; set; } = new();
    public List<ChartPointDto> ChartData { get; set; } = new();
}

public class ChartPointDto
{
    public string Label { get; set; } = null!;
    public decimal Value { get; set; }
}

public class PartnerOrdersFilterDto : PaginationDto
{
    public OrderStatus? Status { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public string? SearchTerm { get; set; }
}

public class PartnerStatsDto
{
    public int OrdersCount { get; set; }
    public int DeliveredCount { get; set; }
    public int FailedCount { get; set; }
    public decimal SuccessRate { get; set; }
    public decimal TotalAmount { get; set; }
    public int AverageDeliveryTime { get; set; }
}

public class PartnerSettingsDto
{
    public bool NotifyOnOrderCreated { get; set; }
    public bool NotifyOnOrderDelivered { get; set; }
    public bool AutoGenerateInvoice { get; set; }
    public int? PreferredSettlementDay { get; set; }
}

public class InvoiceFilterDto : PaginationDto
{
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
    public DateTime IssuedAt { get; set; }
}

public class SettlementFilterDto : PaginationDto
{
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public class SettlementDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
    public DateTime SettledAt { get; set; }
}
