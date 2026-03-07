using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Auth;

namespace Sekka.Core.Validators.Auth;

public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage(ErrorMessages.Required("التوكن"));

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage(ErrorMessages.Required("الـ Refresh Token"));
    }
}
