using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Social;

namespace Sekka.Core.Interfaces.Services;

public interface ISavingsCircleService
{
    Task<Result<CircleDto>> CreateAsync(Guid driverId, CreateCircleDto dto);
    Task<Result<PagedResult<CircleDto>>> GetAvailableAsync(PaginationDto pagination);
    Task<Result<CircleDetailDto>> GetByIdAsync(Guid circleId);
    Task<Result<List<CircleDto>>> GetMyCirclesAsync(Guid driverId);
    Task<Result<bool>> JoinAsync(Guid driverId, Guid circleId);
    Task<Result<bool>> LeaveAsync(Guid driverId, Guid circleId);
    Task<Result<CirclePaymentDto>> MakePaymentAsync(Guid driverId, Guid circleId);
    Task<Result<List<CirclePaymentDto>>> GetPaymentsAsync(Guid circleId);
}
