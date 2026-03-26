using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface IOrderSourceService
{
    Task<Result<OrderSourceTagDto>> GetSourceTagAsync(Guid orderId);
    Task<Result<OrderSourceTagDto>> SetSourceTagAsync(Guid orderId, OrderSourceTagDto dto);
    Task<Result<List<OrderSourceStatsDto>>> GetMonthlyStatsAsync(Guid driverId, int year, int month);
}
