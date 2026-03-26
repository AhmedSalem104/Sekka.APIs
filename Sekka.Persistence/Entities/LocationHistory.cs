namespace Sekka.Persistence.Entities;

public class LocationHistory
{
    public long Id { get; set; }
    public Guid DriverId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Accuracy { get; set; }
    public double? Speed { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Driver Driver { get; set; } = null!;
}
