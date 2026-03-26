using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface IOrderService
{
    Task<Result<OrderDto>> CreateAsync(Guid driverId, CreateOrderDto dto);
    Task<Result<PagedResult<OrderListDto>>> GetOrdersAsync(Guid driverId, OrderFilterDto filter);
    Task<Result<OrderDetailDto>> GetByIdAsync(Guid driverId, Guid orderId);
    Task<Result<OrderDto>> UpdateAsync(Guid driverId, Guid orderId, UpdateOrderDto dto);
    Task<Result<bool>> DeleteAsync(Guid driverId, Guid orderId);
    Task<Result<OrderDto>> UpdateStatusAsync(Guid driverId, Guid orderId, UpdateStatusDto dto);
    Task<Result<OrderDto>> DeliverAsync(Guid driverId, Guid orderId, DeliverOrderDto dto);
    Task<Result<DeliveryAttemptDto>> FailAsync(Guid driverId, Guid orderId, FailOrderDto dto);
    Task<Result<OrderDto>> PartialDeliverAsync(Guid driverId, Guid orderId, PartialDeliveryDto dto);
    Task<Result<OrderPhotoDto>> UploadPhotoAsync(Guid driverId, Guid orderId, Stream fileStream, string fileName, int photoType);
}
