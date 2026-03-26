using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Communication;

public class CreateMessageTemplateDto
{
    public string MessageText { get; set; } = null!;
    public MessageCategory Category { get; set; }
}

public class UpdateMessageTemplateDto
{
    public string? MessageText { get; set; }
    public MessageCategory? Category { get; set; }
    public int? SortOrder { get; set; }
}

public class MessageTemplateDto
{
    public Guid Id { get; set; }
    public string MessageText { get; set; } = null!;
    public MessageCategory Category { get; set; }
    public int UsageCount { get; set; }
    public bool IsSystemTemplate { get; set; }
    public int SortOrder { get; set; }
}
