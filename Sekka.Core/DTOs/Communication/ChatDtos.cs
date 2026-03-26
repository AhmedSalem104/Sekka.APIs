using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Communication;

public class ConversationDto
{
    public Guid Id { get; set; }
    public ChatType ChatType { get; set; }
    public string? Subject { get; set; }
    public bool IsClosed { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = null!;
    public string SenderType { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? AttachmentUrl { get; set; }
    public MessageStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SendMessageDto
{
    public string Content { get; set; } = null!;
    public string? AttachmentUrl { get; set; }
}

public class CreateConversationDto
{
    public ChatType ChatType { get; set; }
    public string? Subject { get; set; }
    public string InitialMessage { get; set; } = null!;
}
