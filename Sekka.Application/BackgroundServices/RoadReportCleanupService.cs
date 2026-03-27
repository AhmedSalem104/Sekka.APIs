using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sekka.Application.BackgroundServices;

public class RoadReportCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RoadReportCleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

    public RoadReportCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<RoadReportCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RoadReportCleanupService started. Interval: {Interval}min", _interval.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredReportsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RoadReportCleanupService");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task CleanupExpiredReportsAsync()
    {
        // TODO: Implement expired road report cleanup
        // 1. Create scope and get IUnitOfWork
        // 2. Query RoadReport where ExpiresAt < DateTime.UtcNow && IsActive == true
        // 3. Set IsActive = false for all expired reports
        // 4. Save changes
        _logger.LogDebug("RoadReportCleanupService tick — cleanup not yet implemented");
        await Task.CompletedTask;
    }
}
