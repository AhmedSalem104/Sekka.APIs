using System.ComponentModel.DataAnnotations;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Profile;

public class UpdateProfileDto
{
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
    public string? Email { get; set; }

    public VehicleType? VehicleType { get; set; }
    public Guid? DefaultRegionId { get; set; }

    [Range(0, 1000000, ErrorMessage = "حد تنبيه الكاش لازم يكون صفر أو أكتر")]
    public decimal? CashAlertThreshold { get; set; }

    public bool? SpeedCompleteMode { get; set; }
}
