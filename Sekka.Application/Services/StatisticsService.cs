using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StatisticsService> _logger;

    public StatisticsService(IUnitOfWork unitOfWork, ILogger<StatisticsService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<DailyStatsDto>> GetDailyAsync(Guid driverId, DateOnly date)
    {
        var repo = _unitOfWork.GetRepository<DailyStats, Guid>();
        var spec = new DailyStatsByDateSpec(driverId, date);
        var stats = await repo.ListAsync(spec);
        var stat = stats.FirstOrDefault();

        if (stat != null)
            return Result<DailyStatsDto>.Success(MapToDto(stat));

        // No pre-aggregated record — calculate on-the-fly
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var dateStart = date.ToDateTime(TimeOnly.MinValue);
        var dateEnd = date.ToDateTime(TimeOnly.MaxValue);
        var orderSpec = new StatDriverOrdersByDateSpec(driverId, dateStart, dateEnd);
        var orders = await orderRepo.ListAsync(orderSpec);

        var expenseRepo = _unitOfWork.GetRepository<Expense, Guid>();
        var expenseSpec = new StatDriverExpensesByDateSpec(driverId, date);
        var expenses = await expenseRepo.ListAsync(expenseSpec);

        var totalOrders = orders.Count;
        var successfulOrders = orders.Count(o => o.Status == OrderStatus.Delivered);
        var failedOrders = orders.Count(o => o.Status == OrderStatus.Failed);
        var cancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled);
        var postponedOrders = orders.Count(o => o.Status == OrderStatus.Postponed);
        var totalEarnings = orders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.Amount);
        var totalCommissions = orders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.CommissionAmount);
        var totalExpenses = expenses.Sum(e => e.Amount);
        var cashCollected = orders.Where(o => o.Status == OrderStatus.Delivered
            && o.PaymentMethod == PaymentMethod.Cash).Sum(o => o.Amount);

        return Result<DailyStatsDto>.Success(new DailyStatsDto
        {
            Id = Guid.Empty,
            DriverId = driverId,
            Date = date,
            TotalOrders = totalOrders,
            SuccessfulOrders = successfulOrders,
            FailedOrders = failedOrders,
            CancelledOrders = cancelledOrders,
            PostponedOrders = postponedOrders,
            TotalEarnings = totalEarnings,
            TotalCommissions = totalCommissions,
            TotalExpenses = totalExpenses,
            NetProfit = totalEarnings - totalCommissions - totalExpenses,
            CashCollected = cashCollected
        });
    }

    public async Task<Result<WeeklyStatsDto>> GetWeeklyAsync(Guid driverId, DateOnly weekStart)
    {
        var weekEnd = weekStart.AddDays(6);
        var repo = _unitOfWork.GetRepository<DailyStats, Guid>();
        var spec = new DailyStatsRangeSpec(driverId, weekStart, weekEnd);
        var stats = await repo.ListAsync(spec);

        var totalOrders = stats.Sum(s => s.TotalOrders);
        var successfulOrders = stats.Sum(s => s.SuccessfulOrders);

        return Result<WeeklyStatsDto>.Success(new WeeklyStatsDto
        {
            WeekStart = weekStart,
            WeekEnd = weekEnd,
            TotalOrders = totalOrders,
            SuccessfulOrders = successfulOrders,
            TotalEarnings = stats.Sum(s => s.TotalEarnings),
            NetProfit = stats.Sum(s => s.NetProfit),
            SuccessRate = totalOrders > 0 ? (double)successfulOrders / totalOrders * 100 : 0,
            DailyBreakdown = stats.Select(MapToDto).ToList()
        });
    }

    public async Task<Result<MonthlyStatsDto>> GetMonthlyAsync(Guid driverId, int month, int year)
    {
        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var repo = _unitOfWork.GetRepository<DailyStats, Guid>();
        var spec = new DailyStatsRangeSpec(driverId, monthStart, monthEnd);
        var stats = await repo.ListAsync(spec);

        var totalOrders = stats.Sum(s => s.TotalOrders);
        var successfulOrders = stats.Sum(s => s.SuccessfulOrders);

        return Result<MonthlyStatsDto>.Success(new MonthlyStatsDto
        {
            Month = month,
            Year = year,
            TotalOrders = totalOrders,
            SuccessfulOrders = successfulOrders,
            TotalEarnings = stats.Sum(s => s.TotalEarnings),
            NetProfit = stats.Sum(s => s.NetProfit),
            SuccessRate = totalOrders > 0 ? (double)successfulOrders / totalOrders * 100 : 0,
            WeeklyBreakdown = new List<WeeklyStatsDto>()
        });
    }

    public async Task<Result<List<HeatmapDataDto>>> GetHeatmapAsync(Guid driverId, DateTime dateFrom, DateTime dateTo)
    {
        var repo = _unitOfWork.GetRepository<DailyStats, Guid>();
        var spec = new DailyStatsRangeSpec(driverId, DateOnly.FromDateTime(dateFrom), DateOnly.FromDateTime(dateTo));
        var stats = await repo.ListAsync(spec);

        // Generate heatmap data points from daily stats
        var heatmap = stats.Select(s => new HeatmapDataDto
        {
            Date = s.Date,
            Hour = 12, // Aggregated daily — no hourly breakdown in DailyStats
            OrderCount = s.TotalOrders,
            Earnings = s.TotalEarnings
        }).ToList();

        return Result<List<HeatmapDataDto>>.Success(heatmap);
    }

    private static DailyStatsDto MapToDto(DailyStats s) => new()
    {
        Id = s.Id,
        DriverId = s.DriverId,
        Date = s.Date,
        TotalOrders = s.TotalOrders,
        SuccessfulOrders = s.SuccessfulOrders,
        FailedOrders = s.FailedOrders,
        CancelledOrders = s.CancelledOrders,
        PostponedOrders = s.PostponedOrders,
        TotalEarnings = s.TotalEarnings,
        TotalCommissions = s.TotalCommissions,
        TotalExpenses = s.TotalExpenses,
        NetProfit = s.NetProfit,
        TotalDistanceKm = s.TotalDistanceKm,
        TimeWorkedMinutes = s.TimeWorkedMinutes,
        CashCollected = s.CashCollected,
        AverageDeliveryTimeMinutes = s.AverageDeliveryTimeMinutes,
        BestRegion = s.BestRegion,
        BestTimeSlot = s.BestTimeSlot
    };
}

internal class DailyStatsByDateSpec : BaseSpecification<DailyStats>
{
    public DailyStatsByDateSpec(Guid driverId, DateOnly date)
    {
        SetCriteria(s => s.DriverId == driverId && s.Date == date);
    }
}

internal class DailyStatsRangeSpec : BaseSpecification<DailyStats>
{
    public DailyStatsRangeSpec(Guid driverId, DateOnly from, DateOnly to)
    {
        SetCriteria(s => s.DriverId == driverId && s.Date >= from && s.Date <= to);
        SetOrderBy(s => s.Date);
    }
}

internal class StatDriverOrdersByDateSpec : BaseSpecification<Order>
{
    public StatDriverOrdersByDateSpec(Guid driverId, DateTime from, DateTime to)
    {
        SetCriteria(o => o.DriverId == driverId && o.CreatedAt >= from && o.CreatedAt <= to);
    }
}

internal class StatDriverExpensesByDateSpec : BaseSpecification<Expense>
{
    public StatDriverExpensesByDateSpec(Guid driverId, DateOnly date)
    {
        SetCriteria(e => e.DriverId == driverId && e.Date == date);
    }
}
