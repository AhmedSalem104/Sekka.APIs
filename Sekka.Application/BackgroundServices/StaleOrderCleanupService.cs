using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.BackgroundServices;

public class StaleOrderCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<StaleOrderCleanupService> _logger;
    private readonly int _staleMinutes;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

    public StaleOrderCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<StaleOrderCleanupService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _staleMinutes = configuration.GetValue<int>("AppSettings:StaleOrderMinutes", 30);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StaleOrderCleanupService started. Interval: {Interval}min, Stale threshold: {Stale}min",
            _interval.TotalMinutes, _staleMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupStaleOrdersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in StaleOrderCleanupService");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task CleanupStaleOrdersAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var repo = unitOfWork.GetRepository<Order, Guid>();

        var cutoff = DateTime.UtcNow.AddMinutes(-_staleMinutes);
        var spec = new StaleOrdersSpec(cutoff);
        var staleOrders = await repo.ListAsync(spec);

        if (!staleOrders.Any()) return;

        foreach (var order in staleOrders)
        {
            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            repo.Update(order);
        }

        await unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Cleaned up {Count} stale orders (pending > {Minutes}min)", staleOrders.Count, _staleMinutes);
    }
}

internal class StaleOrdersSpec : BaseSpecification<Order>
{
    public StaleOrdersSpec(DateTime cutoff)
    {
        SetCriteria(o => o.Status == OrderStatus.Pending && o.CreatedAt < cutoff);
    }
}
