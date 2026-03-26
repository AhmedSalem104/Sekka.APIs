using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class SurgePricingService : ISurgePricingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SurgePricingService> _logger;

    public SurgePricingService(IUnitOfWork unitOfWork, ILogger<SurgePricingService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task<Result<SurgeMultiplierDto>> CalculateMultiplierAsync(Guid regionId, DateTime dateTime)
        => Task.FromResult(Result<SurgeMultiplierDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("التسعير الديناميكي")));
}
