using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(IUnitOfWork unitOfWork, ILogger<AnalyticsService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // ───────── 1. Source Breakdown ─────────
    public async Task<Result<List<SourceBreakdownDto>>> GetSourceBreakdownAsync(
        Guid driverId, DateTime dateFrom, DateTime dateTo)
    {
        var orders = await _unitOfWork.GetRepository<Order, Guid>()
            .ListAsync(new AnalyticsDriverOrdersSpec(driverId, dateFrom, dateTo));

        var totalOrders = orders.Count;
        if (totalOrders == 0)
            return Result<List<SourceBreakdownDto>>.Success(new List<SourceBreakdownDto>());

        var result = orders
            .GroupBy(o => o.SourceType)
            .Select(g =>
            {
                var delivered = g.Where(o => o.Status == OrderStatus.Delivered);
                var revenue = delivered.Sum(o => o.Amount);
                return new SourceBreakdownDto
                {
                    Source = g.Key.ToString(),
                    OrderCount = g.Count(),
                    Revenue = revenue,
                    Percentage = Math.Round((double)g.Count() / totalOrders * 100, 2)
                };
            })
            .OrderByDescending(s => s.Revenue)
            .ToList();

        return Result<List<SourceBreakdownDto>>.Success(result);
    }

    // ───────── 2. Customer Profitability ─────────
    public async Task<Result<List<CustomerProfitabilityDto>>> GetCustomerProfitabilityAsync(
        Guid driverId, DateTime dateFrom, DateTime dateTo)
    {
        var orders = await _unitOfWork.GetRepository<Order, Guid>()
            .ListAsync(new AnalyticsDriverOrdersSpec(driverId, dateFrom, dateTo));

        var withCustomer = orders.Where(o => o.CustomerId.HasValue).ToList();
        if (withCustomer.Count == 0)
            return Result<List<CustomerProfitabilityDto>>.Success(new List<CustomerProfitabilityDto>());

        var result = withCustomer
            .GroupBy(o => new { o.CustomerId, o.CustomerName })
            .Select(g =>
            {
                var total = g.Count();
                var delivered = g.Count(o => o.Status == OrderStatus.Delivered);
                var totalRevenue = g.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.Amount);
                return new CustomerProfitabilityDto
                {
                    CustomerId = g.Key.CustomerId!.Value,
                    CustomerName = g.Key.CustomerName ?? "غير معروف",
                    OrderCount = total,
                    TotalRevenue = totalRevenue,
                    AverageOrderValue = total > 0 ? Math.Round(totalRevenue / total, 2) : 0,
                    SuccessRate = total > 0 ? Math.Round((double)delivered / total * 100, 2) : 0
                };
            })
            .OrderByDescending(c => c.TotalRevenue)
            .ToList();

        return Result<List<CustomerProfitabilityDto>>.Success(result);
    }

    // ───────── 3. Region Analysis ─────────
    public async Task<Result<List<RegionAnalysisDto>>> GetRegionAnalysisAsync(
        Guid driverId, DateTime dateFrom, DateTime dateTo)
    {
        var orders = await _unitOfWork.GetRepository<Order, Guid>()
            .ListAsync(new AnalyticsDriverOrdersSpec(driverId, dateFrom, dateTo));

        if (orders.Count == 0)
            return Result<List<RegionAnalysisDto>>.Success(new List<RegionAnalysisDto>());

        var result = orders
            .GroupBy(o => ExtractRegion(o.DeliveryAddress))
            .Select(g =>
            {
                var total = g.Count();
                var delivered = g.Where(o => o.Status == OrderStatus.Delivered).ToList();
                var deliveredCount = delivered.Count;
                var revenue = delivered.Sum(o => o.Amount);

                var deliveryTimes = delivered
                    .Where(o => o.DeliveredAt.HasValue && o.PickedUpAt.HasValue)
                    .Select(o => (o.DeliveredAt!.Value - o.PickedUpAt!.Value).TotalMinutes)
                    .ToList();

                return new RegionAnalysisDto
                {
                    Region = g.Key,
                    OrderCount = total,
                    Revenue = revenue,
                    AverageDeliveryTimeMinutes = deliveryTimes.Count > 0
                        ? Math.Round(deliveryTimes.Average(), 2) : 0,
                    SuccessRate = total > 0
                        ? Math.Round((double)deliveredCount / total * 100, 2) : 0
                };
            })
            .OrderByDescending(r => r.Revenue)
            .ToList();

        return Result<List<RegionAnalysisDto>>.Success(result);
    }

    // ───────── 4. Time Analysis ─────────
    public async Task<Result<List<TimeAnalysisDto>>> GetTimeAnalysisAsync(
        Guid driverId, DateTime dateFrom, DateTime dateTo)
    {
        var orders = await _unitOfWork.GetRepository<Order, Guid>()
            .ListAsync(new AnalyticsDriverOrdersSpec(driverId, dateFrom, dateTo));

        if (orders.Count == 0)
            return Result<List<TimeAnalysisDto>>.Success(new List<TimeAnalysisDto>());

        var result = orders
            .GroupBy(o => o.CreatedAt.Hour)
            .Select(g =>
            {
                var delivered = g.Where(o => o.Status == OrderStatus.Delivered).ToList();
                var revenue = delivered.Sum(o => o.Amount);

                var deliveryTimes = delivered
                    .Where(o => o.DeliveredAt.HasValue && o.PickedUpAt.HasValue)
                    .Select(o => (o.DeliveredAt!.Value - o.PickedUpAt!.Value).TotalMinutes)
                    .ToList();

                return new TimeAnalysisDto
                {
                    Hour = g.Key,
                    OrderCount = g.Count(),
                    Revenue = revenue,
                    AverageDeliveryTimeMinutes = deliveryTimes.Count > 0
                        ? Math.Round(deliveryTimes.Average(), 2) : 0
                };
            })
            .OrderBy(t => t.Hour)
            .ToList();

        return Result<List<TimeAnalysisDto>>.Success(result);
    }

    // ───────── 5. Cancellation Report ─────────
    public async Task<Result<List<CancellationReportDto>>> GetCancellationReportAsync(
        Guid driverId, DateTime dateFrom, DateTime dateTo)
    {
        var orders = await _unitOfWork.GetRepository<Order, Guid>()
            .ListAsync(new AnalyticsCancelledOrdersSpec(driverId, dateFrom, dateTo));

        if (orders.Count == 0)
            return Result<List<CancellationReportDto>>.Success(new List<CancellationReportDto>());

        var totalCancelled = orders.Count;

        var result = orders
            .GroupBy(o => o.CancellationLog?.CancellationReason ?? CancellationReason.Other)
            .Select(g =>
            {
                var count = g.Count();
                var lostRevenue = g.Sum(o =>
                    o.CancellationLog != null && o.CancellationLog.LossAmount > 0
                        ? o.CancellationLog.LossAmount
                        : o.Amount);

                return new CancellationReportDto
                {
                    Reason = g.Key.ToString(),
                    Count = count,
                    Percentage = Math.Round((double)count / totalCancelled * 100, 2),
                    LostRevenue = lostRevenue
                };
            })
            .OrderByDescending(c => c.Count)
            .ToList();

        return Result<List<CancellationReportDto>>.Success(result);
    }

    // ───────── 6. Profitability Trends ─────────
    public async Task<Result<List<ProfitabilityTrendDto>>> GetProfitabilityTrendsAsync(
        Guid driverId, string period)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        DateOnly rangeStart = period.ToLowerInvariant() switch
        {
            "daily" => now.AddDays(-30),
            "weekly" => now.AddDays(-84),   // 12 weeks
            _ => now.AddMonths(-12)         // monthly (default)
        };

        var statsRepo = _unitOfWork.GetRepository<DailyStats, Guid>();
        var stats = await statsRepo.ListAsync(
            new AnalyticsDailyStatsRecentSpec(driverId, rangeStart, now));

        var expenseRepo = _unitOfWork.GetRepository<Expense, Guid>();
        var expenses = await expenseRepo.ListAsync(
            new AnalyticsExpensesRangeSpec(driverId, rangeStart, now));

        List<ProfitabilityTrendDto> result = period.ToLowerInvariant() switch
        {
            "daily" => BuildDailyTrends(stats, expenses),
            "weekly" => BuildWeeklyTrends(stats, expenses),
            _ => BuildMonthlyTrends(stats, expenses)
        };

        return Result<List<ProfitabilityTrendDto>>.Success(result);
    }

    // ─────────────────── Private helpers ───────────────────

    private static string ExtractRegion(string? address)
    {
        if (string.IsNullOrWhiteSpace(address)) return "غير محدد";
        var parts = address.Split(',', StringSplitOptions.TrimEntries);
        return string.IsNullOrWhiteSpace(parts[0]) ? "غير محدد" : parts[0];
    }

    private static string GetWeekKey(DateOnly date)
    {
        var daysToMonday = ((int)date.DayOfWeek - 1 + 7) % 7;
        var monday = date.AddDays(-daysToMonday);
        return monday.ToString("yyyy-MM-dd");
    }

    private static ProfitabilityTrendDto BuildTrendDto(string periodLabel, decimal revenue, decimal expenses)
    {
        var net = revenue - expenses;
        return new ProfitabilityTrendDto
        {
            Period = periodLabel,
            Revenue = revenue,
            Expenses = expenses,
            NetProfit = net,
            ProfitMargin = revenue != 0 ? Math.Round((double)(net / revenue) * 100, 2) : 0
        };
    }

    private static List<ProfitabilityTrendDto> BuildDailyTrends(
        List<DailyStats> stats, List<Expense> expenses)
    {
        var expByDate = expenses.GroupBy(e => e.Date)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        return stats
            .GroupBy(s => s.Date)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var revenue = g.Sum(s => s.TotalEarnings);
                var exp = (expByDate.TryGetValue(g.Key, out var e) ? e : 0) + g.Sum(s => s.TotalExpenses);
                return BuildTrendDto(g.Key.ToString("yyyy-MM-dd"), revenue, exp);
            })
            .ToList();
    }

    private static List<ProfitabilityTrendDto> BuildWeeklyTrends(
        List<DailyStats> stats, List<Expense> expenses)
    {
        var expByWeek = expenses.GroupBy(e => GetWeekKey(e.Date))
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        return stats
            .GroupBy(s => GetWeekKey(s.Date))
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var revenue = g.Sum(s => s.TotalEarnings);
                var exp = (expByWeek.TryGetValue(g.Key, out var e) ? e : 0) + g.Sum(s => s.TotalExpenses);
                return BuildTrendDto(g.Key, revenue, exp);
            })
            .ToList();
    }

    private static List<ProfitabilityTrendDto> BuildMonthlyTrends(
        List<DailyStats> stats, List<Expense> expenses)
    {
        var expByMonth = expenses.GroupBy(e => e.Date.ToString("yyyy-MM"))
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        return stats
            .GroupBy(s => s.Date.ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var revenue = g.Sum(s => s.TotalEarnings);
                var exp = (expByMonth.TryGetValue(g.Key, out var e) ? e : 0) + g.Sum(s => s.TotalExpenses);
                return BuildTrendDto(g.Key, revenue, exp);
            })
            .ToList();
    }
}

// ─────────────────── Specifications ───────────────────

internal class AnalyticsDriverOrdersSpec : BaseSpecification<Order>
{
    public AnalyticsDriverOrdersSpec(Guid driverId, DateTime from, DateTime to)
    {
        SetCriteria(o => o.DriverId == driverId && o.CreatedAt >= from && o.CreatedAt <= to);
        AsNoTracking = true;
    }
}

internal class AnalyticsCancelledOrdersSpec : BaseSpecification<Order>
{
    public AnalyticsCancelledOrdersSpec(Guid driverId, DateTime from, DateTime to)
    {
        SetCriteria(o => o.DriverId == driverId
                      && o.CreatedAt >= from && o.CreatedAt <= to
                      && o.Status == OrderStatus.Cancelled);
        AddInclude(o => o.CancellationLog!);
        AsNoTracking = true;
    }
}

internal class AnalyticsDailyStatsRecentSpec : BaseSpecification<DailyStats>
{
    public AnalyticsDailyStatsRecentSpec(Guid driverId, DateOnly from, DateOnly to)
    {
        SetCriteria(s => s.DriverId == driverId && s.Date >= from && s.Date <= to);
        SetOrderBy(s => s.Date);
        AsNoTracking = true;
    }
}

internal class AnalyticsExpensesRangeSpec : BaseSpecification<Expense>
{
    public AnalyticsExpensesRangeSpec(Guid driverId, DateOnly from, DateOnly to)
    {
        SetCriteria(e => e.DriverId == driverId && e.Date >= from && e.Date <= to);
        AsNoTracking = true;
    }
}
