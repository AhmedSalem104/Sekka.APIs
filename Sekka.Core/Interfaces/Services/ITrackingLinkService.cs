using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface ITrackingLinkService
{
    Task<Result<TrackingPageDto>> GetTrackingPageAsync(string trackingCode);
    Task<Result<ShareLinkDto>> CreateShareLinkAsync(Guid driverId, Guid orderId, int? ttlMinutes = null);
}
