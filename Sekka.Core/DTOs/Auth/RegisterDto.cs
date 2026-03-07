using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Auth;

public class RegisterDto
{
    public string PhoneNumber { get; set; } = null!;
    public string OtpCode { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string Name { get; set; } = null!;
    public VehicleType VehicleType { get; set; }
    public string? Email { get; set; }
}
