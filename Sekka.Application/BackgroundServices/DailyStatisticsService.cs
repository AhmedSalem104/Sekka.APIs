using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.BackgroundServices;

public class DailyStatisticsService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyStatisticsService> _logger;
    private readonly TimeSpan _runTime; // Time of day to run (default 11 PM)

    public DailyStatisticsService(
        IServiceScopeFactory scopeFactory,
        ILogger<DailyStatisticsService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        var hour = configuration.GetValue<int>("AppSettings:DailyStatsHour", 23);
        _runTime = TimeSpan.FromHours(hour);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DailyStatisticsService started. Scheduled run time: {RunTime}", _runTime);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextRun = now.Date.Add(_runTime);
            if (nextRun <= now)
                nextRun = nextRun.AddDays(1);

            var delay = nextRun - now;
            _logger.LogInformation("DailyStatisticsService next run at {NextRun} (in {Delay})", nextRun, delay);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            try
            {
                await AggregateDailyStatsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DailyStatisticsService aggregation");
            }
        }
    }

    private async Task AggregateDailyStatsAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        _logger.LogInformation("Starting daily statistics aggregation for {Date}", today);

        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Get all active drivers
        var driverRepo = unitOfWork.GetRepository<Driver, Guid>();
        var activeDriversSpec = new ActiveDriversSpec();
        var drivers = await driverRepo.ListAsync(activeDriversSpec);

        var orderRepo = unitOfWork.GetRepository<Order, Guid>();
        var expenseRepo = unitOfWork.GetRepository<Expense, Guid>();
        var settlementRepo = unitOfWork.GetRepository<Settlement, Guid>();
        var statsRepo = unitOfWork.GetRepository<DailyStats, Guid>();

        var aggregatedCount = 0;

        foreach (var driver in drivers)
        {
            try
            {
                // Check if stats already exist for today
                var existingSpec = new DailyStatsExistsSpec(driver.Id, today);
                var existing = await statsRepo.ListAsync(existingSpec);
                if (existing.Any())
                    continue;

                // Get today's orders
                var todayStart = today.ToDateTime(TimeOnly.MinValue);
                var todayEnd = today.ToDateTime(TimeOnly.MaxValue);

                var ordersSpec = new DriverOrdersByDateSpec(driver.Id, todayStart, todayEnd);
                var orders = await orderRepo.ListAsync(ordersSpec);

                // Get today's expenses
                var expensesSpec = new DriverExpensesByDateSpec(driver.Id, today);
                var expenses = await expenseRepo.ListAsync(expensesSpec);

                // Get today's settlements
                var settlementsSpec = new DriverSettlementsByDateSpec(driver.Id, todayStart, todayEnd);
                var settlements = await settlementRepo.ListAsync(settlementsSpec);

                var totalOrders = orders.Count;
                var successfulOrders = orders.Count(o => o.Status == OrderStatus.Delivered);
                var failedOrders = orders.Count(o => o.Status == OrderStatus.Failed);
                var cancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled);
                var totalEarnings = orders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.Amount);
                var totalCommissions = orders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.CommissionAmount);
                var totalExpenses = expenses.Sum(e => e.Amount);
                var cashCollected = orders.Where(o => o.Status == OrderStatus.Delivered && o.PaymentMethod == PaymentMethod.Cash).Sum(o => o.Amount);

                var stats = new DailyStats
                {
                    Id = Guid.NewGuid(),
                    DriverId = driver.Id,
                    Date = today,
                    TotalOrders = totalOrders,
                    SuccessfulOrders = successfulOrders,
                    FailedOrders = failedOrders,
                    CancelledOrders = cancelledOrders,
                    PostponedOrders = orders.Count(o => o.Status == OrderStatus.Postponed),
                    TotalEarnings = totalEarnings,
                    TotalCommissions = totalCommissions,
                    TotalExpenses = totalExpenses,
                    NetProfit = totalEarnings - totalCommissions - totalExpenses,
                    TotalDistanceKm = 0, // Would be calculated from GPS data
                    TimeWorkedMinutes = 0, // Would be calculated from shift data
                    CashCollected = cashCollected,
                    CreatedAt = DateTime.UtcNow
                };

                await statsRepo.AddAsync(stats);
                aggregatedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error aggregating stats for driver {DriverId}", driver.Id);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Daily statistics aggregation complete. Processed {Count} drivers for {Date}", aggregatedCount, today);
    }
}

internal class ActiveDriversSpec : BaseSpecification<Driver>
{
    public ActiveDriversSpec()
    {
        SetCriteria(d => d.IsActive);
    }
}

internal class DailyStatsExistsSpec : BaseSpecification<DailyStats>
{
    public DailyStatsExistsSpec(Guid driverId, DateOnly date)
    {
        SetCriteria(s => s.DriverId == driverId && s.Date == date);
        ApplyPaging(0, 1);
    }
}

internal class DriverOrdersByDateSpec : BaseSpecification<Order>
{
    public DriverOrdersByDateSpec(Guid driverId, DateTime from, DateTime to)
    {
        SetCriteria(o => o.DriverId == driverId && o.CreatedAt >= from && o.CreatedAt <= to);
    }
}

internal class DriverExpensesByDateSpec : BaseSpecification<Expense>
{
    public DriverExpensesByDateSpec(Guid driverId, DateOnly date)
    {
        SetCriteria(e => e.DriverId == driverId && e.Date == date);
    }
}

internal class DriverSettlementsByDateSpec : BaseSpecification<Settlement>
{
    public DriverSettlementsByDateSpec(Guid driverId, DateTime from, DateTime to)
    {
        SetCriteria(s => s.DriverId == driverId && s.SettledAt >= from && s.SettledAt <= to);
    }
}
