namespace Sekka.Core.DTOs.Social;

public class NearbyDriverDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double DistanceKm { get; set; }
    public bool IsAvailable { get; set; }
    public string? VehicleType { get; set; }
}

public class CreateHelpRequestDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string HelpType { get; set; } = null!;
}

public class HelpRequestDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string HelpType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public Guid? ResponderId { get; set; }
    public string? ResponderName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
