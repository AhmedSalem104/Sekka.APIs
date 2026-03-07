namespace Sekka.Core.DTOs.Auth;

public class RefreshTokenDto
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
