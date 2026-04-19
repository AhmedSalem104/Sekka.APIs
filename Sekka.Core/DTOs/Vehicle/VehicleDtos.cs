using System.ComponentModel.DataAnnotations;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Vehicle;

public class CreateVehicleDto
{
    public VehicleType VehicleType { get; set; }

    [MinLength(2, ErrorMessage = "رقم اللوحة لازم يكون حرفين على الأقل")]
    public string? PlateNumber { get; set; }

    public string? MakeModel { get; set; }

    [Range(1990, 2030, ErrorMessage = "سنة الصنع لازم تكون بين 1990 و 2030")]
    public int? Year { get; set; }

    [Range(0, 9999999, ErrorMessage = "عدد الكيلومترات لازم يكون صفر أو أكتر")]
    public double CurrentMileageKm { get; set; }

    [Range(0.01, 100, ErrorMessage = "استهلاك الوقود لازم يكون بين 0.01 و 100")]
    public decimal? FuelConsumptionPer100Km { get; set; }

    [Range(0.01, 1000, ErrorMessage = "سعر الوقود لازم يكون بين 0.01 و 1000")]
    public decimal? FuelPricePerLiter { get; set; }

    public DateOnly? InsuranceExpiryDate { get; set; }
}

public class UpdateVehicleDto
{
    public VehicleType? VehicleType { get; set; }

    [MinLength(2, ErrorMessage = "رقم اللوحة لازم يكون حرفين على الأقل")]
    public string? PlateNumber { get; set; }

    public string? MakeModel { get; set; }

    [Range(1990, 2030, ErrorMessage = "سنة الصنع لازم تكون بين 1990 و 2030")]
    public int? Year { get; set; }

    [Range(0, 9999999, ErrorMessage = "عدد الكيلومترات لازم يكون صفر أو أكتر")]
    public double? CurrentMileageKm { get; set; }

    [Range(0.01, 100, ErrorMessage = "استهلاك الوقود لازم يكون بين 0.01 و 100")]
    public decimal? FuelConsumptionPer100Km { get; set; }

    [Range(0.01, 1000, ErrorMessage = "سعر الوقود لازم يكون بين 0.01 و 1000")]
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
