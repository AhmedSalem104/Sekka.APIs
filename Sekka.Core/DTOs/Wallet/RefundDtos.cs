using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Wallet;

public class CreateRefundDto
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public RefundReason RefundReason { get; set; }
    public string? Description { get; set; }
}

public class RefundDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public decimal Amount { get; set; }
    public RefundReason RefundReason { get; set; }
    public RefundStatus Status { get; set; }
    public string? Description { get; set; }
    public string? AdminNotes { get; set; }
    public string? ProcessedBy { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RefundFilterDto : PaginationDto
{
    public RefundStatus? Status { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public class ApproveRefundDto
{
    public string? Notes { get; set; }
}

public class RejectRefundDto
{
    public string Reason { get; set; } = null!;
}

public class RefundStatsDto
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public decimal TotalRefundedAmount { get; set; }
}
