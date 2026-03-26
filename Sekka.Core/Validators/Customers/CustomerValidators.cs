using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Customer;
using Sekka.Core.DTOs.Partner;
using Sekka.Core.Validators.Common;

namespace Sekka.Core.Validators.Customers;

public class CreateRatingDtoValidator : AbstractValidator<CreateRatingDto>
{
    public CreateRatingDtoValidator()
    {
        RuleFor(x => x.RatingValue).InclusiveBetween(1, 5).WithMessage("التقييم يجب أن يكون بين 1 و 5");
        RuleFor(x => x.FeedbackText).MaximumLength(500).WithMessage(ErrorMessages.MaxLength("التعليق", 500));
    }
}

public class BlockCustomerDtoValidator : AbstractValidator<BlockCustomerDto>
{
    public BlockCustomerDtoValidator()
    {
        RuleFor(x => x.Reason).MaximumLength(200).WithMessage(ErrorMessages.MaxLength("السبب", 200));
    }
}

public class CreateCallerIdNoteDtoValidator : AbstractValidator<CreateCallerIdNoteDto>
{
    public CreateCallerIdNoteDtoValidator()
    {
        RuleFor(x => x.PhoneNumber).MustBeEgyptianMobile();
        RuleFor(x => x.DisplayName).MaximumLength(100).WithMessage(ErrorMessages.MaxLength("الاسم", 100));
        RuleFor(x => x.Note).MaximumLength(500).WithMessage(ErrorMessages.MaxLength("الملاحظة", 500));
    }
}

public class SaveAddressDtoValidator : AbstractValidator<SaveAddressDto>
{
    public SaveAddressDtoValidator()
    {
        RuleFor(x => x.AddressText).NotEmpty().WithMessage(ErrorMessages.Required("العنوان")).MaximumLength(500).WithMessage(ErrorMessages.MaxLength("العنوان", 500));
        RuleFor(x => x.Landmarks).MaximumLength(500).WithMessage(ErrorMessages.MaxLength("العلامات", 500));
        RuleFor(x => x.DeliveryNotes).MaximumLength(500).WithMessage(ErrorMessages.MaxLength("ملاحظات التسليم", 500));
    }
}

public class CreatePartnerDtoValidator : AbstractValidator<CreatePartnerDto>
{
    public CreatePartnerDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ErrorMessages.Required("اسم الشريك")).MaximumLength(100).WithMessage(ErrorMessages.MaxLength("اسم الشريك", 100));
    }
}

public class CreatePickupPointDtoValidator : AbstractValidator<CreatePickupPointDto>
{
    public CreatePickupPointDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ErrorMessages.Required("اسم النقطة")).MaximumLength(100).WithMessage(ErrorMessages.MaxLength("اسم النقطة", 100));
        RuleFor(x => x.Address).NotEmpty().WithMessage(ErrorMessages.Required("العنوان")).MaximumLength(500).WithMessage(ErrorMessages.MaxLength("العنوان", 500));
    }
}
