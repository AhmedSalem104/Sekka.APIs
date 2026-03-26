using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class RecurringOrderService : IRecurringOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RecurringOrderService> _logger;

    public RecurringOrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RecurringOrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<RecurringOrderDto>>> GetRecurringOrdersAsync(Guid driverId)
    {
        _logger.LogInformation("Recurring orders requested by driver {DriverId}", driverId);
        return await Task.FromResult(Result<List<RecurringOrderDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الطلبات المتكررة")));
    }

    public async Task<Result<RecurringOrderDto>> CreateRecurringOrderAsync(Guid driverId, CreateRecurringOrderDto dto)
    {
        _logger.LogInformation("Create recurring order requested by driver {DriverId}", driverId);
        return await Task.FromResult(Result<RecurringOrderDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الطلبات المتكررة")));
    }

    public async Task<Result<RecurringOrderDto>> UpdateRecurringOrderAsync(Guid driverId, Guid orderId, UpdateRecurringOrderDto dto)
    {
        _logger.LogInformation("Update recurring order {OrderId} requested by driver {DriverId}", orderId, driverId);
        return await Task.FromResult(Result<RecurringOrderDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الطلبات المتكررة")));
    }

    public async Task<Result<bool>> PauseAsync(Guid driverId, Guid orderId)
    {
        _logger.LogInformation("Pause recurring order {OrderId} requested by driver {DriverId}", orderId, driverId);
        return await Task.FromResult(Result<bool>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الطلبات المتكررة")));
    }

    public async Task<Result<bool>> ResumeAsync(Guid driverId, Guid orderId)
    {
        _logger.LogInformation("Resume recurring order {OrderId} requested by driver {DriverId}", orderId, driverId);
        return await Task.FromResult(Result<bool>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الطلبات المتكررة")));
    }

    public async Task<Result<bool>> DeleteAsync(Guid driverId, Guid orderId)
    {
        _logger.LogInformation("Delete recurring order {OrderId} requested by driver {DriverId}", orderId, driverId);
        return await Task.FromResult(Result<bool>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الطلبات المتكررة")));
    }
}
