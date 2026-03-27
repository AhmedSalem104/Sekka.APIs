using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.System;

public class AuditLogDto
{
    public long Id { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public AuditAction Action { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
}

public class AuditLogFilterDto : PaginationDto
{
    public string? EntityType { get; set; }
    public Guid? UserId { get; set; }
    public AuditAction? Action { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class AuditActionsSummaryDto
{
    public int TotalActions { get; set; }
    public int CreateCount { get; set; }
    public int UpdateCount { get; set; }
    public int DeleteCount { get; set; }
    public List<EntityActionCount> TopEntities { get; set; } = new();
}

public class EntityActionCount
{
    public string EntityType { get; set; } = null!;
    public int Count { get; set; }
}
