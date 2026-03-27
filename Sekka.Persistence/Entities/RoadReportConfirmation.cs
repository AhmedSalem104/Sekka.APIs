using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class RoadReportConfirmation : BaseEntity<Guid>
{
    public Guid ReportId { get; set; }
    public Guid DriverId { get; set; }
    public bool IsConfirmed { get; set; }

    // Navigation
    public RoadReport Report { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
}
