using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class ShiftService : IShiftService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ShiftService> _logger;

    public ShiftService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ShiftService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<Result<ShiftDto>> StartShiftAsync(Guid driverId, StartShiftDto dto)
    {
        _logger.LogWarning("StartShift called — feature under development");
        return Task.FromResult(Result<ShiftDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("بدء الوردية")));
    }

    public Task<Result<ShiftDto>> EndShiftAsync(Guid driverId)
    {
        _logger.LogWarning("EndShift called — feature under development");
        return Task.FromResult(Result<ShiftDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("إنهاء الوردية")));
    }

    public Task<Result<ShiftDto>> GetCurrentShiftAsync(Guid driverId)
    {
        _logger.LogWarning("GetCurrentShift called — feature under development");
        return Task.FromResult(Result<ShiftDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الوردية الحالية")));
    }

    public Task<Result<ShiftSummaryDto>> GetSummaryAsync(Guid driverId, DateOnly? from, DateOnly? to)
    {
        _logger.LogWarning("GetShiftSummary called — feature under development");
        return Task.FromResult(Result<ShiftSummaryDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("ملخص الورديات")));
    }
}
