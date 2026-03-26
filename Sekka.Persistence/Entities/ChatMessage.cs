using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class ChatMessage : BaseEntity<Guid>
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderType { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? AttachmentUrl { get; set; }
    public MessageStatus Status { get; set; }
    public DateTime? ReadAt { get; set; }

    public Conversation Conversation { get; set; } = null!;
}
