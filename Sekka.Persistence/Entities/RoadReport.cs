using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class RoadReport : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public RoadReportType Type { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Description { get; set; }
    public ReportSeverity Severity { get; set; } = ReportSeverity.Medium;
    public int ConfirmationsCount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ExpiresAt { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public ICollection<RoadReportConfirmation> Confirmations { get; set; } = new List<RoadReportConfirmation>();
}
