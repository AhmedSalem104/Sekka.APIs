using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Wallet;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public string? DriverName { get; set; }
    public string? PartnerName { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
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
    public string Description { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? OrderNumber { get; set; }
}

public class InvoiceFilterDto : PaginationDto
{
    public InvoiceStatus? Status { get; set; }
    public Guid? PartnerId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public class InvoiceSummaryDto
{
    public int TotalInvoices { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingAmount { get; set; }
}

public class GenerateInvoiceDto
{
    public Guid DriverId { get; set; }
    public Guid? PartnerId { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public string? Notes { get; set; }
}

public class BulkGenerateInvoiceDto
{
    public List<Guid> DriverIds { get; set; } = new();
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
}

public class UpdateInvoiceStatusDto
{
    public InvoiceStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class InvoiceStatsDto
{
    public int TotalIssued { get; set; }
    public int TotalPaid { get; set; }
    public int TotalOverdue { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageInvoiceValue { get; set; }
}
