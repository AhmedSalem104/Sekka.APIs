using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class DuplicateDetectionService : IDuplicateDetectionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DuplicateDetectionService> _logger;

    public DuplicateDetectionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DuplicateDetectionService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<DuplicateResultDto>> CheckDuplicateAsync(Guid driverId, CheckDuplicateDto dto)
    {
        _logger.LogInformation("Duplicate check requested by driver {DriverId}", driverId);
        return await Task.FromResult(Result<DuplicateResultDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("كشف التكرار")));
    }
}
