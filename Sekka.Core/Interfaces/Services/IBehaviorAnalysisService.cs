using Sekka.Core.Common;
using Sekka.Core.DTOs.Intelligence;

namespace Sekka.Core.Interfaces.Services;

public interface IBehaviorAnalysisService
{
    Task<Result<CustomerBehaviorSummaryDto>> GetBehaviorSummaryAsync(Guid driverId, Guid customerId);
}
