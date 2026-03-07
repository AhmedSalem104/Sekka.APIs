using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Auth;

namespace Sekka.Core.Validators.Auth;

public class CompleteRegistrationDtoValidator : AbstractValidator<CompleteRegistrationDto>
{
    public CompleteRegistrationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessages.Required("الاسم"))
            .MaximumLength(100).WithMessage(ErrorMessages.MaxLength("الاسم", 100));

        RuleFor(x => x.VehicleType)
            .IsInEnum().WithMessage(ErrorMessages.InvalidFormat("نوع المركبة"));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage(ErrorMessages.InvalidFormat("البريد الإلكتروني"))
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}
