using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.BackgroundServices;

public class CashAlertBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CashAlertBackgroundService> _logger;
    private readonly decimal _defaultThreshold;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);

    public CashAlertBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<CashAlertBackgroundService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _defaultThreshold = configuration.GetValue<decimal>("AppSettings:CashAlertThresholdEGP", 2000);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CashAlertBackgroundService started. Interval: {Interval}min, Default threshold: {Threshold} EGP",
            _interval.TotalMinutes, _defaultThreshold);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckCashAlertsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CashAlertBackgroundService");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task CheckCashAlertsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var driverRepo = unitOfWork.GetRepository<Driver, Guid>();

        var spec = new OnlineDriversSpec();
        var onlineDrivers = await driverRepo.ListAsync(spec);

        var alertCount = 0;
        foreach (var driver in onlineDrivers)
        {
            var threshold = driver.CashAlertThreshold > 0 ? driver.CashAlertThreshold : _defaultThreshold;

            if (driver.CashOnHand >= threshold)
            {
                alertCount++;
                _logger.LogWarning("Cash alert: Driver {DriverName} ({DriverId}) has {Cash} EGP (threshold: {Threshold})",
                    driver.Name, driver.Id, driver.CashOnHand, threshold);
            }
        }

        if (alertCount > 0)
            _logger.LogInformation("Cash alert check complete. {Count} drivers over threshold", alertCount);
    }
}

internal class OnlineDriversSpec : BaseSpecification<Driver>
{
    public OnlineDriversSpec()
    {
        SetCriteria(d => d.IsOnline && d.IsActive);
    }
}
