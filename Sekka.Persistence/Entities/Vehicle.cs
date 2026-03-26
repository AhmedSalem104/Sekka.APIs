using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Vehicle : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public VehicleType VehicleType { get; set; }
    public string? PlateNumber { get; set; }
    public string? MakeModel { get; set; }
    public int? Year { get; set; }
    public double CurrentMileageKm { get; set; }
    public decimal? FuelConsumptionPer100Km { get; set; }
    public decimal? FuelPricePerLiter { get; set; }
    public DateOnly? InsuranceExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
    public VehicleApprovalStatus ApprovalStatus { get; set; }
    public string? RejectionReason { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public Driver Driver { get; set; } = null!;
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
}
