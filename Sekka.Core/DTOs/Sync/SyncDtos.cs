using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Sync;

public class SyncPushDto
{
    public List<SyncChangeDto> Changes { get; set; } = new();
    public DateTime DeviceTimestamp { get; set; }
}

public class SyncChangeDto
{
    public SyncOperation OperationType { get; set; }
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTime LocalTimestamp { get; set; }
}

public class SyncResultDto
{
    public int SyncedCount { get; set; }
    public int ConflictCount { get; set; }
    public int FailedCount { get; set; }
    public List<SyncConflictDto> Conflicts { get; set; } = new();
    public DateTime SyncTimestamp { get; set; }
}

public class SyncConflictDto
{
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public string LocalValue { get; set; } = null!;
    public string ServerValue { get; set; } = null!;
    public string SuggestedResolution { get; set; } = null!;
}

public class SyncConflictResolutionDto
{
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public string Resolution { get; set; } = null!;
    public string? MergedPayload { get; set; }
}

public class SyncPullResultDto
{
    public List<SyncChangeDto> Changes { get; set; } = new();
    public DateTime ServerTimestamp { get; set; }
    public bool HasMore { get; set; }
}

public class SyncStatusDto
{
    public DateTime? LastSyncAt { get; set; }
    public int PendingChanges { get; set; }
    public int ConflictsCount { get; set; }
    public bool IsOnline { get; set; }
}
