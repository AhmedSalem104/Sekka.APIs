using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface ICancellationService
{
    Task<Result<CancellationLogDto>> CancelOrderAsync(Guid driverId, Guid orderId, CancelOrderDto dto);
}
