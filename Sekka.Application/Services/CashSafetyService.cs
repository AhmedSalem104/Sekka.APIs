using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class CashSafetyService : ICashSafetyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CashSafetyService> _logger;

    public CashSafetyService(IUnitOfWork unitOfWork, ILogger<CashSafetyService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task<Result<CashStatusDto>> GetCashStatusAsync(Guid driverId)
        => Task.FromResult(Result<CashStatusDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("أمان النقدية")));
}
