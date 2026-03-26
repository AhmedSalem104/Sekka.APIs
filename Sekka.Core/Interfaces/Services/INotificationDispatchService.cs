using Sekka.Core.Common;

namespace Sekka.Core.Interfaces.Services;

public interface INotificationDispatchService
{
    Task<Result<bool>> DispatchAsync(Guid recipientId, string channel, string subject, string body);
}
