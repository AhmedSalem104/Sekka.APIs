using FluentValidation;
using Sekka.Core.Common;

namespace Sekka.Core.Validators.Common;

public static class EgyptianPhoneValidatorExtensions
{
    /// <summary>
    /// Validates that the phone is a valid Egyptian mobile number (010/011/012/015).
    /// Accepts formats: 01x, +201x, 00201x
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeEgyptianMobile<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(phone => string.IsNullOrEmpty(phone) || EgyptianPhoneHelper.IsMobile(phone))
            .WithMessage("رقم موبايل مصري غير صالح — يجب أن يبدأ بـ 010, 011, 012, أو 015 (11 رقم)")
            .WithErrorCode("INVALID_EGYPTIAN_MOBILE");
    }
}
