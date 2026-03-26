using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface IWaitingTimerService
{
    Task<Result<WaitingTimerDto>> StartTimerAsync(Guid driverId, Guid orderId);
    Task<Result<WaitingTimerDto>> StopTimerAsync(Guid driverId, Guid orderId);
}
