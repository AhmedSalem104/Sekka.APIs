using Sekka.Core.DTOs.Profile;

namespace Sekka.Core.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsNewUser { get; set; }
    public DriverProfileDto Driver { get; set; } = null!;
}
