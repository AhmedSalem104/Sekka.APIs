using Sekka.Core.Common;

namespace Sekka.Core.Interfaces.Services;

public interface IFirebaseService
{
    Task<Result<bool>> SendPushAsync(Guid driverId, string title, string body, Dictionary<string, string>? data = null);
}
