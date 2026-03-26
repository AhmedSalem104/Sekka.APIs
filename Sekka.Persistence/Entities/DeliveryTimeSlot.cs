using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class DeliveryTimeSlot : AuditableEntity<Guid>
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentBookings { get; set; }
    public Guid? RegionId { get; set; }
    public decimal PriceMultiplier { get; set; } = 1.0m;
    public bool IsActive { get; set; } = true;
}
