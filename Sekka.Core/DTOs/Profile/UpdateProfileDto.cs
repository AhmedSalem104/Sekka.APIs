using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Profile;

public class UpdateProfileDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public VehicleType? VehicleType { get; set; }
    public Guid? DefaultRegionId { get; set; }
    public decimal? CashAlertThreshold { get; set; }
    public bool? SpeedCompleteMode { get; set; }
}
