namespace Sekka.Core.DTOs.Auth;

public class SetPasswordDto
{
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
