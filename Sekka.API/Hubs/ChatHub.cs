using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Sekka.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat_{conversationId}");
        await Clients.OthersInGroup($"Chat_{conversationId}").SendAsync("UserJoined", Context.UserIdentifier);
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Chat_{conversationId}");
        await Clients.OthersInGroup($"Chat_{conversationId}").SendAsync("UserLeft", Context.UserIdentifier);
    }

    public async Task SendMessage(string conversationId, string content)
    {
        var senderId = Context.UserIdentifier;
        await Clients.Group($"Chat_{conversationId}").SendAsync("ReceiveMessage", new
        {
            SenderId = senderId,
            Content = content,
            SentAt = DateTime.UtcNow
        });

        // Also notify admin chat group
        await Clients.Group("AdminChat").SendAsync("NewChatMessage", new
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            SentAt = DateTime.UtcNow
        });
    }

    public async Task StartTyping(string conversationId)
    {
        await Clients.OthersInGroup($"Chat_{conversationId}").SendAsync("UserTyping", Context.UserIdentifier);
    }

    public async Task StopTyping(string conversationId)
    {
        await Clients.OthersInGroup($"Chat_{conversationId}").SendAsync("UserStoppedTyping", Context.UserIdentifier);
    }

    // ── Server-to-client methods (called from services via IHubContext<ChatHub>) ──

    // await _hubContext.Clients.Group($"Chat_{conversationId}").SendAsync("ReceiveMessage", message);
    // await _hubContext.Clients.Group($"Chat_{conversationId}").SendAsync("UserTyping", userId);
    // await _hubContext.Clients.Group($"Chat_{conversationId}").SendAsync("UserStoppedTyping", userId);
    // await _hubContext.Clients.Group($"Chat_{conversationId}").SendAsync("ConversationClosed", conversationId);
    // await _hubContext.Clients.Group("AdminChat").SendAsync("NewChatMessage", message);

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            // Admin users join the AdminChat group
            if (Context.User?.IsInRole("Admin") == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "AdminChat");
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User?.IsInRole("Admin") == true)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminChat");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
