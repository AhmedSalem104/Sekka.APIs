using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Sekka.API.Hubs;

[Authorize]
public class OrderTrackingHub : Hub
{
    public async Task JoinOrderGroup(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderId}");
    }

    public async Task LeaveOrderGroup(string orderId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Order_{orderId}");
    }

    public async Task UpdateLocation(double latitude, double longitude)
    {
        var driverId = Context.UserIdentifier;
        await Clients.Group($"Driver_{driverId}").SendAsync("DriverLocationUpdated", new
        {
            DriverId = driverId,
            Latitude = latitude,
            Longitude = longitude,
            Timestamp = DateTime.UtcNow
        });
    }

    [Authorize(Roles = "Admin")]
    public async Task JoinAdminDashboard()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "AdminDashboard");
    }

    public override async Task OnConnectedAsync()
    {
        var driverId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(driverId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Driver_{driverId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var driverId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(driverId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Driver_{driverId}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
