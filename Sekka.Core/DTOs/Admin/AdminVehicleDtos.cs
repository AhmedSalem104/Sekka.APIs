using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Vehicle;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Admin;

public class AdminVehicleFilterDto : PaginationDto
{
    public VehicleType? VehicleType { get; set; }
    public VehicleApprovalStatus? ApprovalStatus { get; set; }
    public bool? MaintenanceDue { get; set; }
    public string? SearchTerm { get; set; }
}

public class AdminVehicleDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public VehicleType VehicleType { get; set; }
    public string? PlateNumber { get; set; }
    public string? MakeModel { get; set; }
    public int? Year { get; set; }
    public bool IsActive { get; set; }
    public VehicleApprovalStatus ApprovalStatus { get; set; }
    public DateOnly? NextMaintenanceDate { get; set; }
    public bool NeedsAttention { get; set; }
}

public class AdminVehicleDetailDto : AdminVehicleDto
{
    public double CurrentMileageKm { get; set; }
    public decimal? FuelConsumptionPer100Km { get; set; }
    public List<MaintenanceRecordDto> MaintenanceHistory { get; set; } = new();
    public decimal TotalMaintenanceCost { get; set; }
}

public class RejectVehicleDto
{
    public string Reason { get; set; } = null!;
}

public class FlagMaintenanceDto
{
    public MaintenanceType MaintenanceType { get; set; }
    public string Urgency { get; set; } = null!;
    public string? Notes { get; set; }
    public DateOnly? DeadlineDate { get; set; }
}

public class VehicleFleetStatsDto
{
    public int TotalVehicles { get; set; }
    public int ActiveVehicles { get; set; }
    public int PendingApproval { get; set; }
    public int MaintenanceDue { get; set; }
    public List<VehicleTypeBreakdownDto> ByType { get; set; } = new();
    public decimal TotalMaintenanceCost { get; set; }
}

public class VehicleTypeBreakdownDto
{
    public VehicleType VehicleType { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}
