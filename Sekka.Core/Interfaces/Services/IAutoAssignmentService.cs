using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface IAutoAssignmentService
{
    Task<Result<List<SuggestedDriverDto>>> GetSuggestedDriversAsync(Guid orderId);
    Task<Result<OrderDto>> AutoAssignAsync(Guid orderId, AssignmentConfigDto config);
}
