using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class OrderWorthService : IOrderWorthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderWorthService> _logger;

    public OrderWorthService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderWorthService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<OrderWorthDto>> CalculateWorthAsync(Guid driverId, Guid orderId)
    {
        _logger.LogInformation("Worth calculation requested for order {OrderId} by driver {DriverId}", orderId, driverId);
        return await Task.FromResult(Result<OrderWorthDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("حساب قيمة الطلب")));
    }

    public async Task<Result<PriceCalculationResultDto>> CalculatePriceAsync(PriceCalculationRequestDto dto)
    {
        _logger.LogInformation("Price calculation requested");
        return await Task.FromResult(Result<PriceCalculationResultDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("حساب السعر")));
    }
}
