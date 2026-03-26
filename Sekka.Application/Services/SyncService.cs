using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Sync;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class SyncService : ISyncService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<SyncService> _logger;

    public SyncService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SyncService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<SyncResultDto>> PushAsync(Guid driverId, SyncPushDto dto)
    {
        _logger.LogInformation("Sync push requested by driver {DriverId}", driverId);
        return await Task.FromResult(Result<SyncResultDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("المزامنة")));
    }

    public async Task<Result<SyncPullResultDto>> PullAsync(Guid driverId, DateTime? lastSyncTimestamp)
    {
        _logger.LogInformation("Sync pull requested by driver {DriverId}, lastSync: {LastSync}", driverId, lastSyncTimestamp);
        return await Task.FromResult(Result<SyncPullResultDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("المزامنة")));
    }

    public async Task<Result<SyncResultDto>> ResolveConflictAsync(Guid driverId, SyncConflictResolutionDto dto)
    {
        _logger.LogInformation("Sync conflict resolution requested by driver {DriverId}", driverId);
        return await Task.FromResult(Result<SyncResultDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("المزامنة")));
    }

    public async Task<Result<SyncStatusDto>> GetStatusAsync(Guid driverId)
    {
        _logger.LogInformation("Sync status requested by driver {DriverId}", driverId);
        return await Task.FromResult(Result<SyncStatusDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("المزامنة")));
    }
}
