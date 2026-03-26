using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Validators.Common;

namespace Sekka.Core.Validators.Orders;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.DeliveryAddress)
            .NotEmpty().WithMessage(ErrorMessages.DeliveryAddressRequired)
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("عنوان التسليم", 500));

        RuleFor(x => x.Amount)
            .InclusiveBetween(0, 999999).WithMessage(ErrorMessages.AmountRange);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("الوصف", 500));

        RuleFor(x => x.PickupAddress)
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("عنوان الاستلام", 500));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage(ErrorMessages.MaxLength("الملاحظات", 1000));

        RuleFor(x => x.RecurrencePattern)
            .MaximumLength(50).WithMessage(ErrorMessages.MaxLength("نمط التكرار", 50));

        RuleFor(x => x.IdempotencyKey)
            .MaximumLength(64).WithMessage(ErrorMessages.MaxLength("مفتاح منع التكرار", 64));

        When(x => !string.IsNullOrEmpty(x.CustomerPhone), () =>
        {
            RuleFor(x => x.CustomerPhone!).MustBeEgyptianMobile();
        });
    }
}

public class UpdateOrderDtoValidator : AbstractValidator<UpdateOrderDto>
{
    public UpdateOrderDtoValidator()
    {
        RuleFor(x => x.Amount)
            .InclusiveBetween(0, 999999).WithMessage(ErrorMessages.AmountRange)
            .When(x => x.Amount.HasValue);

        RuleFor(x => x.DeliveryAddress)
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("عنوان التسليم", 500))
            .When(x => x.DeliveryAddress != null);

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage(ErrorMessages.MaxLength("الملاحظات", 1000))
            .When(x => x.Notes != null);
    }
}

public class DeliverOrderDtoValidator : AbstractValidator<DeliverOrderDto>
{
    public DeliverOrderDtoValidator()
    {
        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("الملاحظات", 500));

        RuleFor(x => x.RatingValue)
            .InclusiveBetween(1, 5).WithMessage("التقييم يجب أن يكون بين 1 و 5")
            .When(x => x.RatingValue.HasValue);
    }
}

public class FailOrderDtoValidator : AbstractValidator<FailOrderDto>
{
    public FailOrderDtoValidator()
    {
        RuleFor(x => x.Reason).IsInEnum();

        RuleFor(x => x.ReasonText)
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("تفاصيل السبب", 500));
    }
}

public class CancelOrderDtoValidator : AbstractValidator<CancelOrderDto>
{
    public CancelOrderDtoValidator()
    {
        RuleFor(x => x.CancellationReason).IsInEnum();

        RuleFor(x => x.ReasonText)
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("تفاصيل السبب", 500));
    }
}

public class CheckDuplicateDtoValidator : AbstractValidator<CheckDuplicateDto>
{
    public CheckDuplicateDtoValidator()
    {
        RuleFor(x => x.CustomerPhone).MustBeEgyptianMobile();

        RuleFor(x => x.DeliveryAddress)
            .NotEmpty().WithMessage(ErrorMessages.DeliveryAddressRequired);
    }
}

public class SwapAddressDtoValidator : AbstractValidator<SwapAddressDto>
{
    public SwapAddressDtoValidator()
    {
        RuleFor(x => x.NewAddress)
            .NotEmpty().WithMessage(ErrorMessages.Required("العنوان الجديد"))
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("العنوان الجديد", 500));

        RuleFor(x => x.Reason)
            .MaximumLength(200).WithMessage(ErrorMessages.MaxLength("السبب", 200));
    }
}

public class BulkImportDtoValidator : AbstractValidator<BulkImportDto>
{
    public BulkImportDtoValidator()
    {
        RuleFor(x => x.RawText)
            .NotEmpty().WithMessage(ErrorMessages.Required("النص"));
    }
}

public class PriceCalculationRequestDtoValidator : AbstractValidator<PriceCalculationRequestDto>
{
    public PriceCalculationRequestDtoValidator()
    {
        RuleFor(x => x.PickupLatitude).NotEmpty();
        RuleFor(x => x.PickupLongitude).NotEmpty();
        RuleFor(x => x.DeliveryLatitude).NotEmpty();
        RuleFor(x => x.DeliveryLongitude).NotEmpty();
    }
}

public class CreateRecurringOrderDtoValidator : AbstractValidator<CreateRecurringOrderDto>
{
    public CreateRecurringOrderDtoValidator()
    {
        RuleFor(x => x.DeliveryAddress)
            .NotEmpty().WithMessage(ErrorMessages.DeliveryAddressRequired);

        RuleFor(x => x.Amount)
            .InclusiveBetween(0, 999999).WithMessage(ErrorMessages.AmountRange);

        RuleFor(x => x.RecurrencePattern)
            .NotEmpty().WithMessage(ErrorMessages.Required("نمط التكرار"))
            .MaximumLength(50).WithMessage(ErrorMessages.MaxLength("نمط التكرار", 50));
    }
}
