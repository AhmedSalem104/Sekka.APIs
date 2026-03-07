using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;
namespace Sekka.Core.DTOs.Admin;
public class AdminSubscriptionFilterDto : PaginationDto
{
    public SubscriptionStatus? Status { get; set; }
    public Guid? PlanId { get; set; }
    public int? ExpiringWithinDays { get; set; }
    public string? SearchTerm { get; set; }
}
public class AdminSubscriptionDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public string DriverPhone { get; set; } = null!;
    public string PlanName { get; set; } = null!;
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysRemaining { get; set; }
    public decimal AmountPaid { get; set; }
    public bool IsGifted { get; set; }
}
public class AdminSubscriptionDetailDto : AdminSubscriptionDto
{
    public List<object> PaymentHistory { get; set; } = new();
    public List<object> PlanChanges { get; set; } = new();
    public string? GiftedBy { get; set; }
}
public class ExtendSubscriptionDto
{
    public int AdditionalDays { get; set; }
    public string Reason { get; set; } = null!;
}
public class CancelSubscriptionDto
{
    public string Reason { get; set; } = null!;
    public decimal? RefundAmount { get; set; }
}
public class ChangeSubscriptionPlanDto
{
    public Guid NewPlanId { get; set; }
    public string Reason { get; set; } = null!;
    public bool AdjustBilling { get; set; } = true;
}
public class GiftSubscriptionDto
{
    public Guid DriverId { get; set; }
    public Guid PlanId { get; set; }
    public int DurationDays { get; set; }
    public string Reason { get; set; } = null!;
}
public class SubscriptionStatsDto
{
    public int TotalActive { get; set; }
    public int TotalExpired { get; set; }
    public int TotalTrial { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRecurringRevenue { get; set; }
    public decimal ChurnRate { get; set; }
    public List<PlanBreakdownDto> PlanBreakdown { get; set; } = new();
}
public class PlanBreakdownDto
{
    public string PlanName { get; set; } = null!;
    public int ActiveCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal Percentage { get; set; }
}
public class SubscriptionPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal PriceMonthly { get; set; }
    public decimal? PriceAnnual { get; set; }
    public List<string> Features { get; set; } = new();
    public bool IsActive { get; set; }
}
public class CreateSubscriptionPlanDto
{
    public string Name { get; set; } = null!;
    public decimal PriceMonthly { get; set; }
    public decimal? PriceAnnual { get; set; }
    public List<string> Features { get; set; } = new();
}
public class UpdateSubscriptionPlanDto
{
    public string? Name { get; set; }
    public decimal? PriceMonthly { get; set; }
    public decimal? PriceAnnual { get; set; }
    public List<string>? Features { get; set; }
}
