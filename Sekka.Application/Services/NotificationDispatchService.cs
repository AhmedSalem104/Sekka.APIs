using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class NotificationDispatchService : INotificationDispatchService
{
    private readonly ILogger<NotificationDispatchService> _logger;

    public NotificationDispatchService(ILogger<NotificationDispatchService> logger)
    {
        _logger = logger;
    }

    public Task<Result<bool>> DispatchAsync(Guid recipientId, string channel, string subject, string body)
    {
        _logger.LogWarning("NotificationDispatchService.DispatchAsync called but feature is under development");
        return Task.FromResult(Result<bool>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("إرسال الإشعارات المتعددة")));
    }
}
