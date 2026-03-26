using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class TrackingLinkService : ITrackingLinkService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TrackingLinkService> _logger;

    public TrackingLinkService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TrackingLinkService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TrackingPageDto>> GetTrackingPageAsync(string trackingCode)
    {
        _logger.LogInformation("Tracking page requested for code {TrackingCode}", trackingCode);
        return await Task.FromResult(Result<TrackingPageDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("رابط التتبع")));
    }
}
