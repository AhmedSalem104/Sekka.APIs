using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Vehicle;

namespace Sekka.Core.Validators.Vehicle;

public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDto>
{
    public CreateVehicleDtoValidator()
    {
        RuleFor(x => x.VehicleType).IsInEnum().WithMessage("نوع المركبة غير صالح");
        RuleFor(x => x.PlateNumber).MaximumLength(20).WithMessage(ErrorMessages.MaxLength("رقم اللوحة", 20));
        RuleFor(x => x.MakeModel).MaximumLength(100).WithMessage(ErrorMessages.MaxLength("الماركة والموديل", 100));
        RuleFor(x => x.Year).InclusiveBetween(2000, 2030).When(x => x.Year.HasValue)
            .WithMessage("سنة الصنع يجب أن تكون بين 2000 و 2030");
        RuleFor(x => x.CurrentMileageKm).GreaterThanOrEqualTo(0).WithMessage("عداد الكيلومترات يجب أن يكون صفر أو أكثر");
        RuleFor(x => x.FuelConsumptionPer100Km).GreaterThan(0).When(x => x.FuelConsumptionPer100Km.HasValue)
            .WithMessage("استهلاك الوقود يجب أن يكون أكبر من صفر");
        RuleFor(x => x.FuelPricePerLiter).GreaterThan(0).When(x => x.FuelPricePerLiter.HasValue)
            .WithMessage("سعر الوقود يجب أن يكون أكبر من صفر");
    }
}

public class CreateMaintenanceDtoValidator : AbstractValidator<CreateMaintenanceDto>
{
    public CreateMaintenanceDtoValidator()
    {
        RuleFor(x => x.MaintenanceType).IsInEnum().WithMessage("نوع الصيانة غير صالح");
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0).When(x => x.Cost.HasValue)
            .WithMessage("التكلفة يجب أن تكون صفر أو أكثر");
        RuleFor(x => x.MileageAtService).GreaterThanOrEqualTo(0).WithMessage("عداد الكيلومترات يجب أن يكون صفر أو أكثر");
        RuleFor(x => x.NextDueMileage).GreaterThan(x => x.MileageAtService).When(x => x.NextDueMileage.HasValue)
            .WithMessage("الكيلومترات القادمة يجب أن تكون أكبر من الحالية");
        RuleFor(x => x.Notes).MaximumLength(500).WithMessage(ErrorMessages.MaxLength("الملاحظات", 500));
    }
}

public class CreateParkingSpotDtoValidator : AbstractValidator<CreateParkingSpotDto>
{
    public CreateParkingSpotDtoValidator()
    {
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).WithMessage("خط العرض يجب أن يكون بين -90 و 90");
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).WithMessage("خط الطول يجب أن يكون بين -180 و 180");
        RuleFor(x => x.Address).MaximumLength(500).WithMessage(ErrorMessages.MaxLength("العنوان", 500));
        RuleFor(x => x.Notes).MaximumLength(500).WithMessage(ErrorMessages.MaxLength("الملاحظات", 500));
        RuleFor(x => x.QualityRating).InclusiveBetween(1, 5).WithMessage("تقييم الجودة يجب أن يكون بين 1 و 5");
        RuleFor(x => x.PaidAmount).GreaterThan(0).When(x => x.IsPaid)
            .WithMessage("المبلغ المدفوع يجب أن يكون أكبر من صفر عند الدفع");
    }
}

public class StartBreakDtoValidator : AbstractValidator<StartBreakDto>
{
    public StartBreakDtoValidator()
    {
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue)
            .WithMessage("خط العرض يجب أن يكون بين -90 و 90");
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue)
            .WithMessage("خط الطول يجب أن يكون بين -180 و 180");
        RuleFor(x => x.LocationDescription).MaximumLength(200).WithMessage(ErrorMessages.MaxLength("وصف الموقع", 200));
        RuleFor(x => x.EnergyBefore).InclusiveBetween(0, 100).When(x => x.EnergyBefore.HasValue)
            .WithMessage("مستوى الطاقة يجب أن يكون بين 0 و 100");
    }
}
