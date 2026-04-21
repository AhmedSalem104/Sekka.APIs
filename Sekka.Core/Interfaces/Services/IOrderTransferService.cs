using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface IOrderTransferService
{
    Task<Result<OrderTransferResponseDto>> TransferAsync(Guid driverId, Guid orderId, TransferOrderDto dto);
    Task<Result<List<TransferRequestDto>>> GetIncomingAsync(Guid driverId);
    Task<Result<List<TransferRequestDto>>> GetOutgoingAsync(Guid driverId);
    Task<Result<OrderTransferResponseDto>> AcceptAsync(Guid driverId, Guid transferId);
    Task<Result<bool>> RejectAsync(Guid driverId, Guid transferId, string? reason);
    Task<Result<bool>> CancelAsync(Guid driverId, Guid transferId);
}
