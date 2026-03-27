using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class SavingsCircleService : ISavingsCircleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<SavingsCircleService> _logger;

    public SavingsCircleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SavingsCircleService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<Result<CircleDto>> CreateAsync(Guid driverId, CreateCircleDto dto)
    {
        _logger.LogWarning("CreateCircle called — feature under development");
        return Task.FromResult(Result<CircleDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("إنشاء حلقة التوفير")));
    }

    public Task<Result<PagedResult<CircleDto>>> GetAvailableAsync(PaginationDto pagination)
    {
        _logger.LogWarning("GetAvailableCircles called — feature under development");
        return Task.FromResult(Result<PagedResult<CircleDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("حلقات التوفير المتاحة")));
    }

    public Task<Result<CircleDetailDto>> GetByIdAsync(Guid circleId)
    {
        _logger.LogWarning("GetCircleById called — feature under development");
        return Task.FromResult(Result<CircleDetailDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("تفاصيل حلقة التوفير")));
    }

    public Task<Result<List<CircleDto>>> GetMyCirclesAsync(Guid driverId)
    {
        _logger.LogWarning("GetMyCircles called — feature under development");
        return Task.FromResult(Result<List<CircleDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("حلقاتي")));
    }

    public Task<Result<bool>> JoinAsync(Guid driverId, Guid circleId)
    {
        _logger.LogWarning("JoinCircle called — feature under development");
        return Task.FromResult(Result<bool>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الانضمام لحلقة التوفير")));
    }

    public Task<Result<bool>> LeaveAsync(Guid driverId, Guid circleId)
    {
        _logger.LogWarning("LeaveCircle called — feature under development");
        return Task.FromResult(Result<bool>.BadRequest(ErrorMessages.FeatureUnderDevelopment("مغادرة حلقة التوفير")));
    }

    public Task<Result<CirclePaymentDto>> MakePaymentAsync(Guid driverId, Guid circleId)
    {
        _logger.LogWarning("MakeCirclePayment called — feature under development");
        return Task.FromResult(Result<CirclePaymentDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("دفع قسط حلقة التوفير")));
    }

    public Task<Result<List<CirclePaymentDto>>> GetPaymentsAsync(Guid circleId)
    {
        _logger.LogWarning("GetCirclePayments called — feature under development");
        return Task.FromResult(Result<List<CirclePaymentDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("مدفوعات حلقة التوفير")));
    }
}
