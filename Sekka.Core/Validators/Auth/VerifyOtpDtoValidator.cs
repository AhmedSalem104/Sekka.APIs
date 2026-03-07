using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Auth;
using Sekka.Core.Validators.Common;

namespace Sekka.Core.Validators.Auth;

public class VerifyOtpDtoValidator : AbstractValidator<VerifyOtpDto>
{
    public VerifyOtpDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ErrorMessages.Required("رقم الهاتف"))
            .MustBeEgyptianMobile();

        RuleFor(x => x.OtpCode)
            .NotEmpty().WithMessage(ErrorMessages.Required("كود التحقق"))
            .Length(4).WithMessage(ErrorMessages.OtpMustBe4Digits);
    }
}
