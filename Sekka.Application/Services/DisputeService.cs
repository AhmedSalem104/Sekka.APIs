using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class DisputeService : IDisputeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DisputeService> _logger;

    public DisputeService(IUnitOfWork unitOfWork, ILogger<DisputeService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task<Result<DisputeDto>> CreateAsync(Guid driverId, CreateDisputeDto dto)
        => Task.FromResult(Result<DisputeDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("النزاعات")));

    public Task<Result<List<DisputeDto>>> GetDisputesAsync(Guid orderId)
        => Task.FromResult(Result<List<DisputeDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("النزاعات")));
}
