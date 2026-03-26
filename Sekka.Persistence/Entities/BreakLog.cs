using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class BreakLog : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationMinutes { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LocationDescription { get; set; }
    public int? EnergyBefore { get; set; }
    public int? EnergyAfter { get; set; }

    public Driver Driver { get; set; } = null!;
}
