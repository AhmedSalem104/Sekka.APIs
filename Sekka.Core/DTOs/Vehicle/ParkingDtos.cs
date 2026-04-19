namespace Sekka.Core.DTOs.Vehicle;

public class CreateParkingSpotDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
    public int QualityRating { get; set; } = 3;
    public bool IsPaid { get; set; }
    public decimal? PaidAmount { get; set; }
    public bool IsShared { get; set; }
}

public class UpdateParkingSpotDto
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
    public int? QualityRating { get; set; }
    public bool? IsPaid { get; set; }
    public decimal? PaidAmount { get; set; }
    public bool? IsShared { get; set; }
}

public class ParkingSpotDto
{
    public Guid Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
    public int QualityRating { get; set; }
    public bool IsPaid { get; set; }
    public decimal? PaidAmount { get; set; }
    public bool IsShared { get; set; }
    public int UsageCount { get; set; }
    public DateTime LastUsedAt { get; set; }
}

public class NearbyQueryDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 1.0;
}
