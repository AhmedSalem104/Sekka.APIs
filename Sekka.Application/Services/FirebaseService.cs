using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class FirebaseService : IFirebaseService
{
    private readonly ILogger<FirebaseService> _logger;

    public FirebaseService(ILogger<FirebaseService> logger)
    {
        _logger = logger;
    }

    public Task<Result<bool>> SendPushAsync(Guid driverId, string title, string body, Dictionary<string, string>? data = null)
    {
        _logger.LogWarning("FirebaseService.SendPushAsync called but feature is under development");
        return Task.FromResult(Result<bool>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("إشعارات Firebase")));
    }
}
