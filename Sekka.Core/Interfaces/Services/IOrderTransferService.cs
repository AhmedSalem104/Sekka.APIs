using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface IOrderTransferService
{
    Task<Result<OrderTransferResponseDto>> TransferAsync(Guid driverId, Guid orderId, TransferOrderDto dto);
}
