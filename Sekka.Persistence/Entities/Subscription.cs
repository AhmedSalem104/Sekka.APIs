using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Subscription : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public Guid PlanId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool AutoRenew { get; set; } = true;
    public Guid? PaymentRequestId { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public SubscriptionPlan Plan { get; set; } = null!;
    public PaymentRequest? PaymentRequest { get; set; }
}
