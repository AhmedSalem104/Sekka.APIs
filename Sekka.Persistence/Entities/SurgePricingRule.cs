using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class SurgePricingRule : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
    public Guid? RegionId { get; set; }
    public SurgeTrigger TriggerType { get; set; }
    public decimal TriggerThreshold { get; set; }
    public decimal PriceMultiplier { get; set; }
    public decimal MaxMultiplier { get; set; } = 3.0m;
    public bool IsActive { get; set; } = true;
    public TimeOnly? ValidFrom { get; set; }
    public TimeOnly? ValidTo { get; set; }
}
