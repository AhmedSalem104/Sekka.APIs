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
        var dailyBreakdown = await BuildDailyBreakdownAsync(driverId, weekStart, weekEnd);

        var totalOrders = dailyBreakdown.Sum(d => d.TotalOrders);
        var successfulOrders = dailyBreakdown.Sum(d => d.SuccessfulOrders);

        return Result<WeeklyStatsDto>.Success(new WeeklyStatsDto
        {
            WeekStart = weekStart,
            WeekEnd = weekEnd,
            TotalOrders = totalOrders,
            SuccessfulOrders = successfulOrders,
            TotalEarnings = dailyBreakdown.Sum(d => d.TotalEarnings),
            NetProfit = dailyBreakdown.Sum(d => d.NetProfit),
            SuccessRate = totalOrders > 0 ? (double)successfulOrders / totalOrders * 100 : 0,
            DailyBreakdown = dailyBreakdown
        });
    }

    public async Task<Result<MonthlyStatsDto>> GetMonthlyAsync(Guid driverId, int month, int year)
    {
        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var allDays = await BuildDailyBreakdownAsync(driverId, monthStart, monthEnd);

        // Build weekly breakdown (7-day chunks)
        var weeklyBreakdown = new List<WeeklyStatsDto>();
        var weekStart = monthStart;
        while (weekStart <= monthEnd)
        {
            var weekEnd = weekStart.AddDays(6);
            if (weekEnd > monthEnd) weekEnd = monthEnd;

            var weekDays = allDays.Where(d => d.Date >= weekStart && d.Date <= weekEnd).ToList();
            var wTotal = weekDays.Sum(d => d.TotalOrders);
            var wSuccess = weekDays.Sum(d => d.SuccessfulOrders);

            weeklyBreakdown.Add(new WeeklyStatsDto
            {
                WeekStart = weekStart,
                WeekEnd = weekEnd,
                TotalOrders = wTotal,
                SuccessfulOrders = wSuccess,
                TotalEarnings = weekDays.Sum(d => d.TotalEarnings),
                NetProfit = weekDays.Sum(d => d.NetProfit),
                SuccessRate = wTotal > 0 ? (double)wSuccess / wTotal * 100 : 0,
                DailyBreakdown = weekDays
            });

            weekStart = weekEnd.AddDays(1);
        }

        var totalOrders = allDays.Sum(d => d.TotalOrders);
        var successfulOrders = allDays.Sum(d => d.SuccessfulOrders);

        return Result<MonthlyStatsDto>.Success(new MonthlyStatsDto
        {
            Month = month,
            Year = year,
            TotalOrders = totalOrders,
            SuccessfulOrders = successfulOrders,
            TotalEarnings = allDays.Sum(d => d.TotalEarnings),
            NetProfit = allDays.Sum(d => d.NetProfit),
            SuccessRate = totalOrders > 0 ? (double)successfulOrders / totalOrders * 100 : 0,
            WeeklyBreakdown = weeklyBreakdown
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

    /// <summary>
    /// Builds daily stats for a date range. Uses DailyStats records where available,
    /// falls back to on-the-fly calculation from orders/expenses for missing days.
    /// </summary>
    private async Task<List<DailyStatsDto>> BuildDailyBreakdownAsync(Guid driverId, DateOnly from, DateOnly to)
    {
        var repo = _unitOfWork.GetRepository<DailyStats, Guid>();
        var spec = new DailyStatsRangeSpec(driverId, from, to);
        var stats = await repo.ListAsync(spec);
        var statsByDate = stats.ToDictionary(s => s.Date);

        // Find dates missing from DailyStats
        var missingDates = new List<DateOnly>();
        for (var d = from; d <= to; d = d.AddDays(1))
        {
            if (!statsByDate.ContainsKey(d)) missingDates.Add(d);
        }

        var calculatedByDate = new Dictionary<DateOnly, DailyStatsDto>();
        if (missingDates.Count > 0)
        {
            // Fetch all orders and expenses for the full range at once (avoid N+1)
            var rangeStart = from.ToDateTime(TimeOnly.MinValue);
            var rangeEnd = to.ToDateTime(TimeOnly.MaxValue);
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var orders = await orderRepo.ListAsync(new StatDriverOrdersByDateSpec(driverId, rangeStart, rangeEnd));

            var expenseRepo = _unitOfWork.GetRepository<Expense, Guid>();
            var expenses = await expenseRepo.ListAsync(new StatDriverExpensesByDateRangeSpec(driverId, from, to));

            var ordersByDate = orders.GroupBy(o => DateOnly.FromDateTime(o.CreatedAt))
                .ToDictionary(g => g.Key, g => g.ToList());
            var expensesByDate = expenses.GroupBy(e => e.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var date in missingDates)
            {
                var dayOrders = ordersByDate.GetValueOrDefault(date) ?? new List<Order>();
                var dayExpenses = expensesByDate.GetValueOrDefault(date) ?? new List<Expense>();
                calculatedByDate[date] = CalculateDailyStats(driverId, date, dayOrders, dayExpenses);
            }
        }

        // Build complete list for all days
        var result = new List<DailyStatsDto>();
        for (var d = from; d <= to; d = d.AddDays(1))
        {
            if (statsByDate.TryGetValue(d, out var s))
                result.Add(MapToDto(s));
            else if (calculatedByDate.TryGetValue(d, out var calc))
                result.Add(calc);
            else
                result.Add(new DailyStatsDto { DriverId = driverId, Date = d });
        }

        return result;
    }

    private static DailyStatsDto CalculateDailyStats(Guid driverId, DateOnly date, List<Order> orders, List<Expense> expenses)
    {
        var delivered = orders.Where(o => o.Status == OrderStatus.Delivered).ToList();
        var totalEarnings = delivered.Sum(o => o.Amount);
        var totalCommissions = delivered.Sum(o => o.CommissionAmount);
        var totalExpenses = expenses.Sum(e => e.Amount);
        var cashCollected = delivered.Where(o => o.PaymentMethod == PaymentMethod.Cash).Sum(o => o.Amount);

        return new DailyStatsDto
        {
            Id = Guid.Empty,
            DriverId = driverId,
            Date = date,
            TotalOrders = orders.Count,
            SuccessfulOrders = delivered.Count,
            FailedOrders = orders.Count(o => o.Status == OrderStatus.Failed),
            CancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled),
            PostponedOrders = orders.Count(o => o.Status == OrderStatus.Postponed),
            TotalEarnings = totalEarnings,
            TotalCommissions = totalCommissions,
            TotalExpenses = totalExpenses,
            NetProfit = totalEarnings - totalCommissions - totalExpenses,
            CashCollected = cashCollected
        };
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

internal class StatDriverExpensesByDateRangeSpec : BaseSpecification<Expense>
{
    public StatDriverExpensesByDateRangeSpec(Guid driverId, DateOnly from, DateOnly to)
    {
        SetCriteria(e => e.DriverId == driverId && e.Date >= from && e.Date <= to);
    }
}
