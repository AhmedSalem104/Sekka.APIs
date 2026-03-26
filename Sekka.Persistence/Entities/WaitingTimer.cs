using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class WaitingTimer : BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Reason { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
}
