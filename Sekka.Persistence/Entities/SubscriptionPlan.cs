using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class SubscriptionPlan : BaseEntity<Guid>
{
    public string Name { get; set; } = null!;
    public string? NameEn { get; set; }
    public decimal PriceMonthly { get; set; }
    public decimal PriceAnnual { get; set; }
    public string Features { get; set; } = null!; // JSON
    public int? MaxOrdersPerDay { get; set; }
    public int? HistoryDays { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    // Navigation
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
