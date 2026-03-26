namespace Sekka.Core.DTOs.Partner;

public class PickupPointDto
{
    public Guid Id { get; set; }
    public string PartnerName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int AverageWaitingMinutes { get; set; }
    public decimal DriverRating { get; set; }
    public int VisitCount { get; set; }
}

public class CreatePickupPointDto
{
    public Guid PartnerId { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
}

public class UpdatePickupPointDto
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
    public bool? IsActive { get; set; }
}

public class RatePickupPointDto
{
    public decimal Rating { get; set; }
    public int? WaitingMinutes { get; set; }
}
