using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class ColleagueRadarService : IColleagueRadarService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ColleagueRadarService> _logger;

    public ColleagueRadarService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ColleagueRadarService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<Result<List<NearbyDriverDto>>> GetNearbyAsync(Guid driverId, double latitude, double longitude, double radiusKm)
    {
        _logger.LogWarning("GetNearbyDrivers called — feature under development");
        return Task.FromResult(Result<List<NearbyDriverDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("رادار الزملاء")));
    }

    public Task<Result<HelpRequestDto>> CreateHelpRequestAsync(Guid driverId, CreateHelpRequestDto dto)
    {
        _logger.LogWarning("CreateHelpRequest called — feature under development");
        return Task.FromResult(Result<HelpRequestDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("طلب مساعدة")));
    }

    public Task<Result<List<HelpRequestDto>>> GetNearbyHelpRequestsAsync(Guid driverId, double latitude, double longitude, double radiusKm)
    {
        _logger.LogWarning("GetNearbyHelpRequests called — feature under development");
        return Task.FromResult(Result<List<HelpRequestDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("طلبات المساعدة القريبة")));
    }

    public Task<Result<HelpRequestDto>> RespondToHelpRequestAsync(Guid driverId, Guid requestId)
    {
        _logger.LogWarning("RespondToHelpRequest called — feature under development");
        return Task.FromResult(Result<HelpRequestDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الاستجابة لطلب المساعدة")));
    }

    public Task<Result<bool>> ResolveHelpRequestAsync(Guid driverId, Guid requestId)
    {
        _logger.LogWarning("ResolveHelpRequest called — feature under development");
        return Task.FromResult(Result<bool>.BadRequest(ErrorMessages.FeatureUnderDevelopment("حل طلب المساعدة")));
    }

    public Task<Result<List<HelpRequestDto>>> GetMyHelpRequestsAsync(Guid driverId)
    {
        _logger.LogWarning("GetMyHelpRequests called — feature under development");
        return Task.FromResult(Result<List<HelpRequestDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("طلبات المساعدة الخاصة بي")));
    }
}
