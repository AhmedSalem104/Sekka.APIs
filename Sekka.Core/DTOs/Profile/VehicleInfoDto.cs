using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Profile;

public class VehicleInfoDto
{
    public Guid Id { get; set; }
    public VehicleType VehicleType { get; set; }
    public string? PlateNumber { get; set; }
    public string? MakeModel { get; set; }
    public string? Color { get; set; }
}
