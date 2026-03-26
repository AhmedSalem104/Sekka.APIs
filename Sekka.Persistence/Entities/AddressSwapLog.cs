using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class AddressSwapLog : BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public string OldAddress { get; set; } = null!;
    public double? OldLatitude { get; set; }
    public double? OldLongitude { get; set; }
    public string NewAddress { get; set; } = null!;
    public double? NewLatitude { get; set; }
    public double? NewLongitude { get; set; }
    public string? Reason { get; set; }
    public double? DistanceDifferenceKm { get; set; }
    public int? TimeDifferenceMinutes { get; set; }
    public decimal? CostDifference { get; set; }
    public DateTime SwappedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Order Order { get; set; } = null!;
}
