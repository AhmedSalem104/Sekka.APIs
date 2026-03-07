using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Profile;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public string PlanName { get; set; } = null!;
    public decimal PriceMonthly { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool AutoRenew { get; set; }
    public List<string> Features { get; set; } = new();
    public int DaysRemaining { get; set; }
}

public class UpgradeSubscriptionDto
{
    public Guid PlanId { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public string BillingCycle { get; set; } = "monthly";
}
