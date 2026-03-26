using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Financial;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public Guid DriverId { get; set; }
    public Guid? PartnerId { get; set; }
    public string? PartnerName { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? PdfUrl { get; set; }
    public string? Notes { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
}

public class InvoiceItemDto
{
    public Guid Id { get; set; }
    public Guid? OrderId { get; set; }
    public string Description { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class InvoiceFilterDto : PaginationDto
{
    public InvoiceStatus? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class InvoiceSummaryDto
{
    public int TotalInvoices { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalPending { get; set; }
}

public class AdminInvoiceFilterDto : PaginationDto
{
    public string? Search { get; set; }
    public Guid? DriverId { get; set; }
    public InvoiceStatus? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class CreateInvoiceDto
{
    public Guid DriverId { get; set; }
    public Guid? PartnerId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string? Notes { get; set; }
}

public class UpdateInvoiceStatusDto
{
    public InvoiceStatus Status { get; set; }
    public string? Notes { get; set; }
}
