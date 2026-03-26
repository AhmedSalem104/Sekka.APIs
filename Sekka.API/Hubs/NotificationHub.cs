using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Sekka.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public async Task JoinDriverGroup(string driverId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Notifications_{driverId}");
    }

    public async Task MarkAsRead(string notificationId)
    {
        var driverId = Context.UserIdentifier;
        await Clients.Group($"Notifications_{driverId}").SendAsync("NotificationRead", notificationId);
    }

    public async Task MarkAllAsRead()
    {
        var driverId = Context.UserIdentifier;
        await Clients.Group($"Notifications_{driverId}").SendAsync("AllNotificationsRead");
    }

    // ── Server-to-client methods (called from services) ──

    // await _hubContext.Clients.Group($"Notifications_{driverId}").SendAsync("NewNotification", notification);
    // await _hubContext.Clients.Group($"Notifications_{driverId}").SendAsync("NotificationRead", notificationId);
    // await _hubContext.Clients.All.SendAsync("BroadcastMessage", message);
    // await _hubContext.Clients.Group($"Notifications_{driverId}").SendAsync("SettlementApproved", settlement);
    // await _hubContext.Clients.Group($"Notifications_{driverId}").SendAsync("PaymentApproved", payment);

    public override async Task OnConnectedAsync()
    {
        var driverId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(driverId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Notifications_{driverId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var driverId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(driverId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Notifications_{driverId}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
