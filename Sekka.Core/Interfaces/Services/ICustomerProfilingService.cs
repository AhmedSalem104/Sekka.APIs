using Sekka.Core.Common;
using Sekka.Core.DTOs.Intelligence;

namespace Sekka.Core.Interfaces.Services;

public interface ICustomerProfilingService
{
    Task<Result<CustomerInterestProfileDto>> GetProfileAsync(Guid driverId, Guid customerId);
}
