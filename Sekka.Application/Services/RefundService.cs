using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class RefundService : IRefundService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RefundService> _logger;

    public RefundService(IUnitOfWork unitOfWork, ILogger<RefundService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task<Result<RefundDto>> CreateAsync(Guid driverId, CreateRefundDto dto)
        => Task.FromResult(Result<RefundDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الاسترداد")));

    public Task<Result<List<RefundDto>>> GetRefundsAsync(Guid orderId)
        => Task.FromResult(Result<List<RefundDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الاسترداد")));
}
