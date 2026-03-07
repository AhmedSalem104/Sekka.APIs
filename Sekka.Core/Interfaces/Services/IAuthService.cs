using Sekka.Core.Common;
using Sekka.Core.DTOs.Auth;

namespace Sekka.Core.Interfaces.Services;

public interface IAuthService
{
    Task<Result<bool>> SendVerificationAsync(ForgotPasswordDto dto);
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto dto);
    Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto dto);
    Task<Result<bool>> ChangePasswordAsync(Guid driverId, ChangePasswordDto dto);
    Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
    Task<Result<bool>> LogoutAsync(Guid driverId, string token);
    Task<Result<bool>> RegisterDeviceAsync(Guid driverId, RegisterDeviceDto dto);
}
