using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Auth;

public class RegisterDeviceDto
{
    public string Token { get; set; } = null!;
    public DevicePlatform Platform { get; set; }
}
