using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class AutoAssignmentService : IAutoAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AutoAssignmentService> _logger;

    public AutoAssignmentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AutoAssignmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<SuggestedDriverDto>>> GetSuggestedDriversAsync(Guid orderId)
    {
        _logger.LogInformation("Suggested drivers requested for order {OrderId}", orderId);
        return await Task.FromResult(Result<List<SuggestedDriverDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("التعيين التلقائي")));
    }

    public async Task<Result<OrderDto>> AutoAssignAsync(Guid orderId, AssignmentConfigDto config)
    {
        _logger.LogInformation("Auto-assign requested for order {OrderId}", orderId);
        return await Task.FromResult(Result<OrderDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("التعيين التلقائي")));
    }
}
