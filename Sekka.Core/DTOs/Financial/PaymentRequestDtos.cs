using System.ComponentModel.DataAnnotations;
using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Financial;

public class PaymentRequestDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public PaymentPurpose PaymentPurpose { get; set; }
    public decimal Amount { get; set; }
    public ManualPaymentMethod PaymentMethod { get; set; }
    public string ReferenceCode { get; set; } = null!;
    public string? ProofImageUrl { get; set; }
    public string? SenderPhone { get; set; }
    public string? SenderName { get; set; }
    public string? Notes { get; set; }
    public PaymentRequestStatus Status { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePaymentRequestDto
{
    public PaymentPurpose PaymentPurpose { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "المبلغ لازم يكون أكبر من صفر")]
    public decimal Amount { get; set; }

    public ManualPaymentMethod PaymentMethod { get; set; }
    public string? SenderPhone { get; set; }
    public string? SenderName { get; set; }
    public string? Notes { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
}

public class PaymentRequestFilterDto : PaginationDto
{
    public PaymentRequestStatus? Status { get; set; }
    public PaymentPurpose? Purpose { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class AdminPaymentFilterDto : PaginationDto
{
    public string? Search { get; set; }
    public Guid? DriverId { get; set; }
    public PaymentRequestStatus? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class ReviewPaymentDto
{
    public PaymentRequestStatus Status { get; set; }
    public string? AdminNotes { get; set; }
}
