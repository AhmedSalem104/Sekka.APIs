namespace Sekka.Core.DTOs.Auth;

public class VerifyOtpDto
{
    public string PhoneNumber { get; set; } = null!;
    public string OtpCode { get; set; } = null!;
}
