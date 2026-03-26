using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Wallet;

public class CreatePaymentRequestDto
{
    public Guid PlanId { get; set; }
    public ManualPaymentMethod PaymentMethod { get; set; }
    public string? SenderPhone { get; set; }
    public string? SenderName { get; set; }
    public string? Notes { get; set; }
}

public class PaymentRequestDto
{
    public Guid Id { get; set; }
    public string ReferenceCode { get; set; } = null!;
    public decimal Amount { get; set; }
    public ManualPaymentMethod PaymentMethod { get; set; }
    public PaymentRequestStatus Status { get; set; }
    public string? ProofImageUrl { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

public class PaymentRequestFilterDto : PaginationDto
{
    public PaymentRequestStatus? Status { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public class ReviewPaymentRequestDto
{
    public PaymentRequestStatus Decision { get; set; }
    public string? AdminNotes { get; set; }
}
