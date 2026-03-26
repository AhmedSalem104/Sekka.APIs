using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Vehicle;

public class CreateVehicleDto
{
    public VehicleType VehicleType { get; set; }
    public string? PlateNumber { get; set; }
    public string? MakeModel { get; set; }
    public int? Year { get; set; }
    public double CurrentMileageKm { get; set; }
    public decimal? FuelConsumptionPer100Km { get; set; }
    public decimal? FuelPricePerLiter { get; set; }
    public DateOnly? InsuranceExpiryDate { get; set; }
}

public class UpdateVehicleDto
{
    public VehicleType? VehicleType { get; set; }
    public string? PlateNumber { get; set; }
    public string? MakeModel { get; set; }
    public int? Year { get; set; }
    public double? CurrentMileageKm { get; set; }
    public decimal? FuelConsumptionPer100Km { get; set; }
    public decimal? FuelPricePerLiter { get; set; }
    public DateOnly? InsuranceExpiryDate { get; set; }
}

public class VehicleDto
{
    public Guid Id { get; set; }
    public VehicleType VehicleType { get; set; }
    public string? PlateNumber { get; set; }
    public string? MakeModel { get; set; }
    public int? Year { get; set; }
    public double CurrentMileageKm { get; set; }
    public decimal? FuelConsumptionPer100Km { get; set; }
    public DateOnly? InsuranceExpiryDate { get; set; }
    public DateOnly? NextMaintenanceDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreateMaintenanceDto
{
    public MaintenanceType MaintenanceType { get; set; }
    public decimal? Cost { get; set; }
    public double MileageAtService { get; set; }
    public double? NextDueMileage { get; set; }
    public DateOnly? NextDueDate { get; set; }
    public string? Notes { get; set; }
}

public class MaintenanceRecordDto
{
    public Guid Id { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public decimal? Cost { get; set; }
    public double MileageAtService { get; set; }
    public double? NextDueMileage { get; set; }
    public DateOnly? NextDueDate { get; set; }
    public string? Notes { get; set; }
    public DateTime ServicedAt { get; set; }
}
