using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface IRecurringOrderService
{
    Task<Result<List<RecurringOrderDto>>> GetRecurringOrdersAsync(Guid driverId);
    Task<Result<RecurringOrderDto>> CreateRecurringOrderAsync(Guid driverId, CreateRecurringOrderDto dto);
    Task<Result<RecurringOrderDto>> UpdateRecurringOrderAsync(Guid driverId, Guid orderId, UpdateRecurringOrderDto dto);
    Task<Result<bool>> PauseAsync(Guid driverId, Guid orderId);
    Task<Result<bool>> ResumeAsync(Guid driverId, Guid orderId);
    Task<Result<bool>> DeleteAsync(Guid driverId, Guid orderId);
}
