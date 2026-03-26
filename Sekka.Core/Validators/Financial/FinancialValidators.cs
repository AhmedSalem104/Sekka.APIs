using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;

namespace Sekka.Core.Validators.Financial;

public class CreateSettlementDtoValidator : AbstractValidator<CreateSettlementDto>
{
    public CreateSettlementDtoValidator()
    {
        RuleFor(x => x.PartnerId)
            .NotEmpty().WithMessage(ErrorMessages.Required("الشريك"));

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("المبلغ يجب أن يكون أكبر من صفر")
            .LessThanOrEqualTo(999999).WithMessage(ErrorMessages.AmountRange);

        RuleFor(x => x.SettlementType)
            .IsInEnum().WithMessage(ErrorMessages.InvalidFormat("نوع التسوية"));

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("الملاحظات", 500));
    }
}

public class CreatePaymentRequestDtoValidator : AbstractValidator<CreatePaymentRequestDto>
{
    public CreatePaymentRequestDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("المبلغ يجب أن يكون أكبر من صفر")
            .LessThanOrEqualTo(999999).WithMessage(ErrorMessages.AmountRange);

        RuleFor(x => x.PaymentPurpose)
            .IsInEnum().WithMessage(ErrorMessages.InvalidFormat("غرض الدفع"));

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage(ErrorMessages.InvalidFormat("طريقة الدفع"));

        RuleFor(x => x.SenderName)
            .MaximumLength(100).WithMessage(ErrorMessages.MaxLength("اسم المرسل", 100));

        RuleFor(x => x.SenderPhone)
            .MaximumLength(20).WithMessage(ErrorMessages.MaxLength("رقم المرسل", 20));

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("الملاحظات", 500));
    }
}

public class CreateExpenseDtoValidator : AbstractValidator<CreateExpenseDto>
{
    public CreateExpenseDtoValidator()
    {
        RuleFor(x => x.ExpenseType)
            .IsInEnum().WithMessage(ErrorMessages.InvalidFormat("نوع المصروف"));

        RuleFor(x => x.Amount)
            .InclusiveBetween(0, 999999).WithMessage(ErrorMessages.AmountRange);

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("الملاحظات", 500));
    }
}

public class WalletAdjustmentDtoValidator : AbstractValidator<WalletAdjustmentDto>
{
    public WalletAdjustmentDtoValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage(ErrorMessages.Required("السائق"));

        RuleFor(x => x.Amount)
            .NotEqual(0).WithMessage("المبلغ يجب أن لا يساوي صفر")
            .InclusiveBetween(-999999, 999999).WithMessage(ErrorMessages.AmountRange);

        RuleFor(x => x.AdjustmentType)
            .IsInEnum().WithMessage(ErrorMessages.InvalidFormat("نوع التعديل"));

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage(ErrorMessages.Required("السبب"))
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("السبب", 500));
    }
}

public class CreateDisputeDtoValidator : AbstractValidator<CreateDisputeDto>
{
    public CreateDisputeDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage(ErrorMessages.Required("الطلب"));

        RuleFor(x => x.DisputeType)
            .IsInEnum().WithMessage(ErrorMessages.InvalidFormat("نوع النزاع"));

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ErrorMessages.Required("الوصف"))
            .MaximumLength(1000).WithMessage(ErrorMessages.MaxLength("الوصف", 1000));

        RuleFor(x => x.EvidenceUrls)
            .MaximumLength(2000).WithMessage(ErrorMessages.MaxLength("روابط الأدلة", 2000));
    }
}

public class CreateRefundDtoValidator : AbstractValidator<CreateRefundDto>
{
    public CreateRefundDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage(ErrorMessages.Required("الطلب"));

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("المبلغ مطلوب ويجب أن يكون أكبر من صفر")
            .LessThanOrEqualTo(999999).WithMessage(ErrorMessages.AmountRange);

        RuleFor(x => x.RefundReason)
            .IsInEnum().WithMessage(ErrorMessages.InvalidFormat("سبب الاسترداد"));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage(ErrorMessages.MaxLength("الوصف", 1000));
    }
}
