using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Wallet;

public class CreateDisputeDto
{
    public Guid OrderId { get; set; }
    public DisputeType DisputeType { get; set; }
    public string Description { get; set; } = null!;
    public List<string>? EvidenceUrls { get; set; }
}

public class DisputeDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public DisputeType DisputeType { get; set; }
    public DisputeStatus Status { get; set; }
    public string Description { get; set; } = null!;
    public List<string>? EvidenceUrls { get; set; }
    public string? AdminNotes { get; set; }
    public string? Resolution { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DisputeFilterDto : PaginationDto
{
    public DisputeStatus? Status { get; set; }
    public DisputeType? DisputeType { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public class ResolveDisputeDto
{
    public string Resolution { get; set; } = null!;
    public string? Notes { get; set; }
}

public class RejectDisputeDto
{
    public string Reason { get; set; } = null!;
}

public class DisputeStatsDto
{
    public int TotalDisputes { get; set; }
    public int OpenDisputes { get; set; }
    public int ResolvedDisputes { get; set; }
    public int RejectedDisputes { get; set; }
    public double AverageResolutionTimeHours { get; set; }
}
