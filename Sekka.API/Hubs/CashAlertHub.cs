using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Sekka.API.Hubs;

[Authorize]
public class CashAlertHub : Hub
{
    public async Task AcknowledgeAlert(string alertId)
    {
        var driverId = Context.UserIdentifier;
        await Clients.Group($"CashAlert_{driverId}").SendAsync("AlertAcknowledged", alertId);
    }

    // ── Server-to-client methods (called from CashAlertBackgroundService / services) ──

    // await _hubContext.Clients.Group($"CashAlert_{driverId}").SendAsync("CashThresholdExceeded", cashStatus);
    // await _hubContext.Clients.Group($"CashAlert_{driverId}").SendAsync("SettlementReminder", reminder);
    // await _hubContext.Clients.Group($"CashAlert_{driverId}").SendAsync("DailySettlementSummary", summary);
    // await _hubContext.Clients.Group($"CashAlert_{driverId}").SendAsync("DepositConfirmed", deposit);

    public override async Task OnConnectedAsync()
    {
        var driverId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(driverId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"CashAlert_{driverId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var driverId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(driverId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"CashAlert_{driverId}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
