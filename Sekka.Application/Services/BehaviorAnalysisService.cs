using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Intelligence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class BehaviorAnalysisService : IBehaviorAnalysisService
{
    private readonly ILogger<BehaviorAnalysisService> _logger;

    public BehaviorAnalysisService(ILogger<BehaviorAnalysisService> logger)
    {
        _logger = logger;
    }

    public Task<Result<CustomerBehaviorSummaryDto>> GetBehaviorSummaryAsync(Guid driverId, Guid customerId)
        => Task.FromResult(Result<CustomerBehaviorSummaryDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("تحليل سلوك العميل")));
}
