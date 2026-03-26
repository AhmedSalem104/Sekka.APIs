using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class ParkingSpot : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
    public int QualityRating { get; set; } = 3;
    public bool IsPaid { get; set; }
    public decimal? PaidAmount { get; set; }
    public int UsageCount { get; set; } = 1;
    public bool IsShared { get; set; }
    public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;

    public Driver Driver { get; set; } = null!;
}
