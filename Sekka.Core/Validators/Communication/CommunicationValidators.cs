using FluentValidation;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Communication;

namespace Sekka.Core.Validators.Communication;

public class ActivateSOSDtoValidator : AbstractValidator<ActivateSOSDto>
{
    public ActivateSOSDtoValidator()
    {
        RuleFor(x => x.Latitude)
            .NotEmpty().WithMessage(ErrorMessages.Required("خط العرض"))
            .InclusiveBetween(-90, 90).WithMessage("خط العرض يجب أن يكون بين -90 و 90");

        RuleFor(x => x.Longitude)
            .NotEmpty().WithMessage(ErrorMessages.Required("خط الطول"))
            .InclusiveBetween(-180, 180).WithMessage("خط الطول يجب أن يكون بين -180 و 180");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("الملاحظات", 500));
    }
}

public class CreateMessageTemplateDtoValidator : AbstractValidator<CreateMessageTemplateDto>
{
    public CreateMessageTemplateDtoValidator()
    {
        RuleFor(x => x.MessageText)
            .NotEmpty().WithMessage(ErrorMessages.Required("نص الرسالة"))
            .MaximumLength(500).WithMessage(ErrorMessages.MaxLength("نص الرسالة", 500));

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage(ErrorMessages.Required("التصنيف"));
    }
}

public class SendMessageDtoValidator : AbstractValidator<SendMessageDto>
{
    public SendMessageDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(ErrorMessages.Required("محتوى الرسالة"))
            .MaximumLength(2000).WithMessage(ErrorMessages.MaxLength("محتوى الرسالة", 2000));

        RuleFor(x => x.AttachmentUrl)
            .MaximumLength(1000).WithMessage(ErrorMessages.MaxLength("رابط المرفق", 1000))
            .When(x => x.AttachmentUrl != null);
    }
}

public class CreateConversationDtoValidator : AbstractValidator<CreateConversationDto>
{
    public CreateConversationDtoValidator()
    {
        RuleFor(x => x.ChatType)
            .NotEmpty().WithMessage(ErrorMessages.Required("نوع المحادثة"));

        RuleFor(x => x.InitialMessage)
            .NotEmpty().WithMessage(ErrorMessages.Required("الرسالة الأولى"))
            .MaximumLength(2000).WithMessage(ErrorMessages.MaxLength("الرسالة الأولى", 2000));

        RuleFor(x => x.Subject)
            .MaximumLength(200).WithMessage(ErrorMessages.MaxLength("الموضوع", 200))
            .When(x => x.Subject != null);
    }
}
