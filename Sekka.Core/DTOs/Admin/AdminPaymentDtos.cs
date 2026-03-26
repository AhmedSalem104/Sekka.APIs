using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Wallet;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Admin;

public class AdminPaymentFilterDto : PaginationDto
{
    public PaymentRequestStatus? Status { get; set; }
    public Guid? DriverId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public class AdminPaymentRequestDto : PaymentRequestDto
{
    public string DriverName { get; set; } = null!;
    public string DriverPhone { get; set; } = null!;
    public string PlanName { get; set; } = null!;
}

public class PaymentStatsDto
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public decimal TotalApprovedAmount { get; set; }
    public double AvgReviewTimeMinutes { get; set; }
}
