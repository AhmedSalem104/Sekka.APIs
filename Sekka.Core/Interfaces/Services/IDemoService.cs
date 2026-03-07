using Sekka.Core.Common;
using Sekka.Core.DTOs.Auth;
using Sekka.Core.DTOs.Demo;

namespace Sekka.Core.Interfaces.Services;

public interface IDemoService
{
    Task<Result<DemoSessionDto>> StartDemoAsync();
    Task<Result<DemoDataDto>> GetDemoDataAsync(Guid sessionId);
    Task<Result<bool>> EndDemoAsync(Guid sessionId);
    Task<Result<AuthResponseDto>> ConvertToRealAccountAsync(Guid sessionId, CompleteRegistrationDto dto);
}
