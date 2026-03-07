using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Auth;

public class CompleteRegistrationDto
{
    public string Name { get; set; } = null!;
    public VehicleType VehicleType { get; set; }
    public string? Email { get; set; }
    public string? ProfileImageUrl { get; set; }
}
