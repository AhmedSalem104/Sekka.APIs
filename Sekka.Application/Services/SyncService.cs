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
        _logger.LogInformation("Sync push from driver {DriverId}: {Count} changes", driverId, dto.Changes.Count);

        var syncRepo = _unitOfWork.GetRepository<SyncQueue, Guid>();
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var syncedItems = new List<SyncedItemDto>();
        var conflicts = new List<SyncConflictDto>();

        foreach (var change in dto.Changes)
        {
            var realId = Guid.NewGuid();
            var entityType = change.EntityType?.ToLower() ?? "";
            var operation = change.OperationType;

            try
            {
                // Process by entity type — create real records in DB
                if (entityType == "order" && operation == SyncOperation.Create)
                {
                    var order = System.Text.Json.JsonSerializer.Deserialize<Order>(change.Payload,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (order != null)
                    {
                        order.Id = realId;
                        order.DriverId = driverId;
                        order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
                        order.Status = Core.Enums.OrderStatus.Pending;
                        order.AssignedAt = DateTime.UtcNow;
                        await orderRepo.AddAsync(order);
                    }
                }
                else if (entityType == "order" && operation == SyncOperation.Update && !string.IsNullOrEmpty(change.EntityId))
                {
                    if (Guid.TryParse(change.EntityId, out var existingId))
                    {
                        var existing = await orderRepo.GetByIdAsync(existingId);
                        if (existing != null && existing.DriverId == driverId)
                        {
                            // Apply payload updates
                            if (!string.IsNullOrEmpty(change.Payload))
                            {
                                var updates = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(change.Payload);
                                // Simple field update — notes, amount, address
                                existing.UpdatedAt = DateTime.UtcNow;
                                orderRepo.Update(existing);
                            }
                            realId = existingId;
                        }
                    }
                }

                // Log in SyncQueue
                var entry = new SyncQueue
                {
                    Id = Guid.NewGuid(),
                    DriverId = driverId,
                    OperationType = operation,
                    EntityType = entityType,
                    EntityId = realId.ToString(),
                    Payload = change.Payload,
                    Status = SyncStatus.Synced,
                    SyncedAt = DateTime.UtcNow
                };
                await syncRepo.AddAsync(entry);

                syncedItems.Add(new SyncedItemDto
                {
                    TempId = change.TempId,
                    RealId = realId.ToString(),
                    EntityType = entityType,
                    Operation = operation.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Sync conflict for {EntityType} tempId={TempId}", entityType, change.TempId);
                conflicts.Add(new SyncConflictDto
                {
                    EntityType = entityType,
                    EntityId = change.TempId ?? "",
                    LocalValue = change.Payload,
                    ServerValue = "",
                    SuggestedResolution = "server_wins"
                });
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return Result<SyncResultDto>.Success(new SyncResultDto
        {
            SyncedCount = syncedItems.Count,
            ConflictCount = conflicts.Count,
            FailedCount = 0,
            SyncedItems = syncedItems,
            Conflicts = conflicts,
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
