using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Sync;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

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
        _logger.LogInformation("Sync push requested by driver {DriverId} with {Count} changes", driverId, dto.Changes.Count);

        var repo = _unitOfWork.GetRepository<SyncQueue, Guid>();
        var syncedCount = 0;

        foreach (var change in dto.Changes)
        {
            var entry = new SyncQueue
            {
                Id = Guid.NewGuid(),
                DriverId = driverId,
                OperationType = change.OperationType,
                EntityType = change.EntityType,
                EntityId = change.EntityId,
                Payload = change.Payload,
                Status = SyncStatus.Synced,
                SyncedAt = DateTime.UtcNow
            };
            await repo.AddAsync(entry);
            syncedCount++;
        }

        await _unitOfWork.SaveChangesAsync();

        return Result<SyncResultDto>.Success(new SyncResultDto
        {
            SyncedCount = syncedCount,
            ConflictCount = 0,
            FailedCount = 0,
            Conflicts = new List<SyncConflictDto>(),
            SyncTimestamp = DateTime.UtcNow
        });
    }

    public async Task<Result<SyncPullResultDto>> PullAsync(Guid driverId, DateTime? lastSyncTimestamp)
    {
        _logger.LogInformation("Sync pull requested by driver {DriverId}, lastSync: {LastSync}", driverId, lastSyncTimestamp);

        var repo = _unitOfWork.GetRepository<SyncQueue, Guid>();
        var spec = new SyncQueueByDriverSpec(driverId, lastSyncTimestamp);
        var entries = await repo.ListAsync(spec);

        var changes = entries.Select(e => new SyncChangeDto
        {
            OperationType = e.OperationType,
            EntityType = e.EntityType,
            EntityId = e.EntityId,
            Payload = e.Payload,
            LocalTimestamp = e.SyncedAt ?? e.CreatedAt
        }).ToList();

        return Result<SyncPullResultDto>.Success(new SyncPullResultDto
        {
            Changes = changes,
            ServerTimestamp = DateTime.UtcNow,
            HasMore = false
        });
    }

    public async Task<Result<SyncResultDto>> ResolveConflictAsync(Guid driverId, SyncConflictResolutionDto dto)
    {
        _logger.LogInformation("Sync conflict resolution requested by driver {DriverId} for {EntityType}/{EntityId}",
            driverId, dto.EntityType, dto.EntityId);

        var repo = _unitOfWork.GetRepository<SyncQueue, Guid>();
        var spec = new SyncQueueConflictSpec(driverId, dto.EntityType, dto.EntityId);
        var entries = await repo.ListAsync(spec);

        foreach (var entry in entries)
        {
            entry.Status = SyncStatus.Synced;
            entry.ConflictResolution = dto.Resolution;
            if (!string.IsNullOrEmpty(dto.MergedPayload))
                entry.Payload = dto.MergedPayload;
            entry.SyncedAt = DateTime.UtcNow;
            repo.Update(entry);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result<SyncResultDto>.Success(new SyncResultDto
        {
            SyncedCount = entries.Count,
            ConflictCount = 0,
            FailedCount = 0,
            Conflicts = new List<SyncConflictDto>(),
            SyncTimestamp = DateTime.UtcNow
        });
    }

    public async Task<Result<SyncStatusDto>> GetStatusAsync(Guid driverId)
    {
        _logger.LogInformation("Sync status requested by driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<SyncQueue, Guid>();

        var allSpec = new SyncQueueByDriverSpec(driverId);
        var all = await repo.ListAsync(allSpec);

        var pendingSpec = new SyncQueuePendingSpec(driverId);
        var pending = await repo.ListAsync(pendingSpec);

        var conflictSpec = new SyncQueueConflictsOnlySpec(driverId);
        var conflicts = await repo.ListAsync(conflictSpec);

        var lastSynced = all
            .Where(e => e.SyncedAt.HasValue)
            .OrderByDescending(e => e.SyncedAt)
            .FirstOrDefault();

        return Result<SyncStatusDto>.Success(new SyncStatusDto
        {
            LastSyncAt = lastSynced?.SyncedAt,
            PendingChanges = pending.Count,
            ConflictsCount = conflicts.Count,
            IsOnline = true
        });
    }
}

internal class SyncQueueByDriverSpec : BaseSpecification<SyncQueue>
{
    public SyncQueueByDriverSpec(Guid driverId, DateTime? since = null)
    {
        SetCriteria(s => s.DriverId == driverId
            && s.Status == SyncStatus.Synced
            && (!since.HasValue || s.SyncedAt > since.Value));
        SetOrderByDescending(s => s.CreatedAt);
    }
}

internal class SyncQueuePendingSpec : BaseSpecification<SyncQueue>
{
    public SyncQueuePendingSpec(Guid driverId)
    {
        SetCriteria(s => s.DriverId == driverId && s.Status == SyncStatus.Pending);
    }
}

internal class SyncQueueConflictsOnlySpec : BaseSpecification<SyncQueue>
{
    public SyncQueueConflictsOnlySpec(Guid driverId)
    {
        SetCriteria(s => s.DriverId == driverId && s.Status == SyncStatus.Conflict);
    }
}

internal class SyncQueueConflictSpec : BaseSpecification<SyncQueue>
{
    public SyncQueueConflictSpec(Guid driverId, string entityType, string entityId)
    {
        SetCriteria(s => s.DriverId == driverId
            && s.EntityType == entityType
            && s.EntityId == entityId
            && s.Status == SyncStatus.Conflict);
    }
}
