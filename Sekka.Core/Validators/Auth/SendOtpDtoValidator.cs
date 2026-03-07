using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Auth;
using Sekka.Core.Validators.Common;

namespace Sekka.Core.Validators.Auth;

public class SendOtpDtoValidator : AbstractValidator<SendOtpDto>
{
    public SendOtpDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ErrorMessages.Required("رقم الهاتف"))
            .MustBeEgyptianMobile();
    }
}
