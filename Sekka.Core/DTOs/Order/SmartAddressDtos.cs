namespace Sekka.Core.DTOs.Order;

public class SmartAddressDto
{
    public string FormattedAddress { get; set; } = null!;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? Governorate { get; set; }
    public double Confidence { get; set; }
}
