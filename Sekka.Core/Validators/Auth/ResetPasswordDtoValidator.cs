using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Auth;
using Sekka.Core.Validators.Common;

namespace Sekka.Core.Validators.Auth;

public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ErrorMessages.Required("رقم الهاتف"))
            .MustBeEgyptianMobile();

        RuleFor(x => x.OtpCode)
            .NotEmpty().WithMessage(ErrorMessages.Required("كود التحقق"))
            .Length(4).WithMessage(ErrorMessages.OtpMustBe4Digits);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(ErrorMessages.Required("كلمة المرور الجديدة"))
            .MinimumLength(6).WithMessage(ErrorMessages.MinLength("كلمة المرور", 6));

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage(ErrorMessages.PasswordMismatch);
    }
}
