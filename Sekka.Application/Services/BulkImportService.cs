using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class BulkImportService : IBulkImportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BulkImportService> _logger;

    public BulkImportService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BulkImportService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<BulkImportResultDto>> ImportAsync(Guid driverId, BulkImportDto dto)
    {
        _logger.LogInformation("Bulk import requested by driver {DriverId}", driverId);
        return await Task.FromResult(Result<BulkImportResultDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الاستيراد المجمّع")));
    }
}
