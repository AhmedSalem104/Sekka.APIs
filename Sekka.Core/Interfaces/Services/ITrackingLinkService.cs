using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface ITrackingLinkService
{
    Task<Result<TrackingPageDto>> GetTrackingPageAsync(string trackingCode);
}
