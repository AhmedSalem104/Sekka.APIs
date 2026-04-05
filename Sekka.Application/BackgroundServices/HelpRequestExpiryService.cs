using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Persistence.Entities;

namespace Sekka.Application.BackgroundServices;

public class HelpRequestExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HelpRequestExpiryService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

    public HelpRequestExpiryService(
        IServiceScopeFactory scopeFactory,
        ILogger<HelpRequestExpiryService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HelpRequestExpiryService started. Interval: {Interval}min", _interval.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExpireStaleRequestsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HelpRequestExpiryService");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task ExpireStaleRequestsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var repo = unitOfWork.GetRepository<FieldAssistanceRequest, Guid>();

        var pendingCutoff = DateTime.UtcNow.AddHours(-2);
        var acceptedCutoff = DateTime.UtcNow.AddHours(-4);

        var spec = new Services.ActiveHelpRequestsSpec(pendingCutoff, acceptedCutoff);
        var staleRequests = await repo.ListAsync(spec);

        if (!staleRequests.Any()) return;

        foreach (var request in staleRequests)
        {
            request.Status = AssistanceStatus.Expired;
            repo.Update(request);
        }

        await unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Expired {Count} stale help requests (Pending > 2h or Accepted > 4h)", staleRequests.Count);
    }
}
