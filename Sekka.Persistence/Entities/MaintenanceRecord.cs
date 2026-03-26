using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class MaintenanceRecord : BaseEntity<Guid>
{
    public Guid VehicleId { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public decimal? Cost { get; set; }
    public double MileageAtService { get; set; }
    public double? NextDueMileage { get; set; }
    public DateOnly? NextDueDate { get; set; }
    public string? Notes { get; set; }
    public DateTime ServicedAt { get; set; } = DateTime.UtcNow;

    public Vehicle Vehicle { get; set; } = null!;
}
