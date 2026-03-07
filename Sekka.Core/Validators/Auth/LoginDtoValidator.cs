using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Auth;
using Sekka.Core.Validators.Common;

namespace Sekka.Core.Validators.Auth;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ErrorMessages.Required("رقم الهاتف"))
            .MustBeEgyptianMobile();

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorMessages.Required("كلمة المرور"));
    }
}
