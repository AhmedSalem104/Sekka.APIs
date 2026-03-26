using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class PickupPoint : BaseEntity<Guid>
{
    public Guid PartnerId { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int AverageWaitingMinutes { get; set; }
    public decimal DriverRating { get; set; }
    public int VisitCount { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Partner Partner { get; set; } = null!;
}
