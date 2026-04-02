using Sekka.Core.Common;

namespace Sekka.Core.Interfaces.Services;

public interface IFirebaseService
{
    /// <summary>Send push notification to a specific driver by DriverId</summary>
    Task<Result<bool>> SendPushAsync(Guid driverId, string title, string body, Dictionary<string, string>? data = null);

    /// <summary>Send push notification to multiple drivers</summary>
    Task<Result<int>> SendPushToManyAsync(IEnumerable<Guid> driverIds, string title, string body, Dictionary<string, string>? data = null);

    /// <summary>Send push notification to all active drivers (broadcast)</summary>
    Task<Result<int>> SendBroadcastAsync(string title, string body, Dictionary<string, string>? data = null);

    /// <summary>Register a device FCM token for a driver</summary>
    Task<Result<bool>> RegisterTokenAsync(Guid driverId, string token, string platform);

    /// <summary>Remove/deactivate a device FCM token</summary>
    Task<Result<bool>> RemoveTokenAsync(Guid driverId, string token);
}
