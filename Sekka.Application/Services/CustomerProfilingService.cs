using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Intelligence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class CustomerProfilingService : ICustomerProfilingService
{
    private readonly ILogger<CustomerProfilingService> _logger;

    public CustomerProfilingService(ILogger<CustomerProfilingService> logger)
    {
        _logger = logger;
    }

    public Task<Result<CustomerInterestProfileDto>> GetProfileAsync(Guid driverId, Guid customerId)
        => Task.FromResult(Result<CustomerInterestProfileDto>.NotImplemented(ErrorMessages.FeatureUnderDevelopment("ملف العميل التفصيلي")));
}
