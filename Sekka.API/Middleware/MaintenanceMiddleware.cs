using System.Text.Json;
using Sekka.Core.Common;

namespace Sekka.API.Middleware;

public class MaintenanceMiddleware
{
    private readonly RequestDelegate _next;

    public MaintenanceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip health check endpoints
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        // TODO: Check maintenance window from AppConfigurations table or Redis flag
        // For now, pass through
        await _next(context);
    }
}
