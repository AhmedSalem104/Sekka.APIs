using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Profile;
using Sekka.Core.Validators.Common;

namespace Sekka.Core.Validators.Emergency;

public class CreateEmergencyContactDtoValidator : AbstractValidator<CreateEmergencyContactDto>
{
    public CreateEmergencyContactDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessages.Required("اسم جهة الطوارئ"))
            .MaximumLength(100);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage(ErrorMessages.Required("رقم جهة الطوارئ"))
            .MustBeEgyptianMobile();
    }
}
