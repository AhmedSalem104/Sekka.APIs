using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Financial;

public class RefundDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid DriverId { get; set; }
    public decimal Amount { get; set; }
    public RefundReason RefundReason { get; set; }
    public RefundStatus Status { get; set; }
    public string? Description { get; set; }
    public string? AdminNotes { get; set; }
    public string? ProcessedBy { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateRefundDto
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public RefundReason RefundReason { get; set; }
    public string? Description { get; set; }
}

public class AdminRefundFilterDto : PaginationDto
{
    public string? Search { get; set; }
    public Guid? DriverId { get; set; }
    public RefundStatus? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class ReviewRefundDto
{
    public RefundStatus Status { get; set; }
    public string? AdminNotes { get; set; }
}
