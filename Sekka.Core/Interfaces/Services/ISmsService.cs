using Sekka.Core.Common;

namespace Sekka.Core.Interfaces.Services;

public interface ISmsService
{
    Task<Result<bool>> SendOtpAsync(string phoneNumber);
    Task<Result<bool>> VerifyOtpAsync(string phoneNumber, string otpCode);
}
