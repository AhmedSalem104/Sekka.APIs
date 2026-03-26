using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Financial;

public class DisputeDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid DriverId { get; set; }
    public DisputeType DisputeType { get; set; }
    public DisputeStatus Status { get; set; }
    public string Description { get; set; } = null!;
    public string? EvidenceUrls { get; set; }
    public string? AdminNotes { get; set; }
    public string? Resolution { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateDisputeDto
{
    public Guid OrderId { get; set; }
    public DisputeType DisputeType { get; set; }
    public string Description { get; set; } = null!;
    public string? EvidenceUrls { get; set; }
}

public class AdminDisputeFilterDto : PaginationDto
{
    public string? Search { get; set; }
    public Guid? DriverId { get; set; }
    public DisputeStatus? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class ResolveDisputeDto
{
    public DisputeStatus Status { get; set; }
    public string Resolution { get; set; } = null!;
    public string? AdminNotes { get; set; }
}
