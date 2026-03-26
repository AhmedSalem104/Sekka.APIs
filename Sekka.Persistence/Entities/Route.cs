using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Route : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public string OrderIds { get; set; } = null!;
    public string? OptimizedSequence { get; set; }
    public int? EstimatedTimeMinutes { get; set; }
    public double? TotalDistanceKm { get; set; }
    public double? ActualDistanceKm { get; set; }
    public decimal? EfficiencyScore { get; set; }
    public double? StartLatitude { get; set; }
    public double? StartLongitude { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? CompletedAt { get; set; }

    public Driver Driver { get; set; } = null!;
}
