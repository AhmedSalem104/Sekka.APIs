using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Auth;
using Sekka.Core.Validators.Common;

namespace Sekka.Core.Validators.Auth;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ErrorMessages.Required("رقم الهاتف"))
            .MustBeEgyptianMobile();

        RuleFor(x => x.OtpCode)
            .NotEmpty().WithMessage(ErrorMessages.Required("كود التحقق"))
            .Length(4).WithMessage(ErrorMessages.OtpMustBe4Digits);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorMessages.Required("كلمة المرور"))
            .MinimumLength(6).WithMessage(ErrorMessages.MinLength("كلمة المرور", 6));

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage(ErrorMessages.PasswordMismatch);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessages.Required("الاسم"))
            .MaximumLength(100).WithMessage(ErrorMessages.MaxLength("الاسم", 100));
    }
}
