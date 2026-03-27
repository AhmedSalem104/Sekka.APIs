using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Intelligence;

namespace Sekka.Core.Interfaces.Services;

public interface IInterestEngineService
{
    Task<Result<bool>> RecordSignalAsync(Guid driverId, Guid customerId, string signalType, string? categoryId, string? metadata);
    Task<Result<CustomerInterestProfileDto>> GetCustomerProfileAsync(Guid driverId, Guid customerId);
    Task<Result<List<CustomerInterestDto>>> GetCustomerInterestsAsync(Guid driverId, Guid customerId);
    Task<Result<List<InterestCategorySummaryDto>>> GetTopInterestsAsync(Guid driverId, TopInterestsQueryDto query);
    Task<Result<List<CustomerSegmentSummaryDto>>> GetSegmentsAsync(Guid driverId);
    Task<Result<PagedResult<CustomerInterestProfileDto>>> GetSegmentCustomersAsync(Guid driverId, Guid segmentId, PaginationDto pagination);
}
