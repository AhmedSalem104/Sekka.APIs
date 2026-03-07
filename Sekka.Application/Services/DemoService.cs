using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Auth;
using Sekka.Core.DTOs.Demo;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class DemoService : IDemoService
{
    private readonly ILogger<DemoService> _logger;

    public DemoService(ILogger<DemoService> logger)
    {
        _logger = logger;
    }

    public Task<Result<DemoSessionDto>> StartDemoAsync()
    {
        var sessionId = Guid.NewGuid();
        _logger.LogInformation("Demo session started: {SessionId}", sessionId);

        return Task.FromResult(Result<DemoSessionDto>.Success(new DemoSessionDto
        {
            SessionId = sessionId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            DemoDriverId = Guid.NewGuid(),
            Token = $"demo_{sessionId}"
        }));
    }

    public Task<Result<DemoDataDto>> GetDemoDataAsync(Guid sessionId)
    {
        return Task.FromResult(Result<DemoDataDto>.Success(new DemoDataDto
        {
            RemainingMinutes = 15
        }));
    }

    public Task<Result<bool>> EndDemoAsync(Guid sessionId)
    {
        _logger.LogInformation("Demo session ended: {SessionId}", sessionId);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public Task<Result<AuthResponseDto>> ConvertToRealAccountAsync(Guid sessionId, CompleteRegistrationDto dto)
    {
        // TODO: Convert demo session to real account
        return Task.FromResult(Result<AuthResponseDto>.BadRequest(ErrorMessages.DemoConvertUnderDev));
    }
}
