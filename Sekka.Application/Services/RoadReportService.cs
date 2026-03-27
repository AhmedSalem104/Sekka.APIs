using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class RoadReportService : IRoadReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RoadReportService> _logger;

    public RoadReportService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoadReportService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<Result<RoadReportDto>> CreateAsync(Guid driverId, CreateRoadReportDto dto)
    {
        _logger.LogWarning("CreateRoadReport called — feature under development");
        return Task.FromResult(Result<RoadReportDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("بلاغات الطريق")));
    }

    public Task<Result<List<RoadReportDto>>> GetNearbyAsync(double latitude, double longitude, double radiusKm)
    {
        _logger.LogWarning("GetNearbyRoadReports called — feature under development");
        return Task.FromResult(Result<List<RoadReportDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("بلاغات الطريق القريبة")));
    }

    public Task<Result<bool>> ConfirmAsync(Guid driverId, Guid reportId, bool isConfirmed)
    {
        _logger.LogWarning("ConfirmRoadReport called — feature under development");
        return Task.FromResult(Result<bool>.BadRequest(ErrorMessages.FeatureUnderDevelopment("تأكيد بلاغ الطريق")));
    }

    public Task<Result<RoadReportDto>> GetByIdAsync(Guid reportId)
    {
        _logger.LogWarning("GetRoadReportById called — feature under development");
        return Task.FromResult(Result<RoadReportDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("تفاصيل بلاغ الطريق")));
    }

    public Task<Result<List<RoadReportDto>>> GetMyReportsAsync(Guid driverId)
    {
        _logger.LogWarning("GetMyRoadReports called — feature under development");
        return Task.FromResult(Result<List<RoadReportDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("بلاغاتي")));
    }

    public Task<Result<bool>> DeactivateAsync(Guid driverId, Guid reportId)
    {
        _logger.LogWarning("DeactivateRoadReport called — feature under development");
        return Task.FromResult(Result<bool>.BadRequest(ErrorMessages.FeatureUnderDevelopment("إلغاء بلاغ الطريق")));
    }
}
