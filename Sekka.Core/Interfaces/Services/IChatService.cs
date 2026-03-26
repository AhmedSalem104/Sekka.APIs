using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Communication;

namespace Sekka.Core.Interfaces.Services;

public interface IChatService
{
    Task<Result<PagedResult<ConversationDto>>> GetConversationsAsync(Guid driverId, PaginationDto pagination);
    Task<Result<ConversationDto>> CreateConversationAsync(Guid driverId, CreateConversationDto dto);
    Task<Result<PagedResult<ChatMessageDto>>> GetMessagesAsync(Guid driverId, Guid conversationId, PaginationDto pagination);
    Task<Result<ChatMessageDto>> SendMessageAsync(Guid driverId, Guid conversationId, SendMessageDto dto);
    Task<Result<bool>> CloseConversationAsync(Guid driverId, Guid conversationId);
    Task<Result<bool>> MarkMessageReadAsync(Guid driverId, Guid messageId);
    Task<Result<int>> GetUnreadCountAsync(Guid driverId);
}
