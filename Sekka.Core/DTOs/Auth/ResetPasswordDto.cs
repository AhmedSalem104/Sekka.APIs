namespace Sekka.Core.DTOs.Auth;

public class ResetPasswordDto
{
    public string PhoneNumber { get; set; } = null!;
    public string OtpCode { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
