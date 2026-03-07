using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class DataDeletionRequest : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public string RequestType { get; set; } = null!;
    public DeletionRequestStatus Status { get; set; }
    public string? DataExportUrl { get; set; }
    public string? ProcessedBy { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
}
