using Sekka.Core.Common;
using Sekka.Core.DTOs.Sync;

namespace Sekka.Core.Interfaces.Services;

public interface ISyncService
{
    Task<Result<SyncResultDto>> PushAsync(Guid driverId, SyncPushDto dto);
    Task<Result<SyncPullResultDto>> PullAsync(Guid driverId, DateTime? lastSyncTimestamp);
    Task<Result<SyncResultDto>> ResolveConflictAsync(Guid driverId, SyncConflictResolutionDto dto);
    Task<Result<SyncStatusDto>> GetStatusAsync(Guid driverId);
}
