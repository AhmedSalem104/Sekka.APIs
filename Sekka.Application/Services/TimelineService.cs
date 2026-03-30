using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class TimelineService : ITimelineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TimelineService> _logger;

    public TimelineService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TimelineService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<DailyTimelineDto>> GetDailyAsync(Guid driverId, DateOnly date)
    {
        _logger.LogInformation("Daily timeline requested by driver {DriverId} for date {Date}", driverId, date);

        var timeline = await BuildDailyTimelineAsync(driverId, date);
        return Result<DailyTimelineDto>.Success(timeline);
    }

    public async Task<Result<List<DailyTimelineDto>>> GetRangeAsync(Guid driverId, DateOnly dateFrom, DateOnly dateTo)
    {
        _logger.LogInformation("Timeline range requested by driver {DriverId} from {DateFrom} to {DateTo}", driverId, dateFrom, dateTo);

        var timelines = new List<DailyTimelineDto>();
        var current = dateFrom;
        while (current <= dateTo)
        {
            var daily = await BuildDailyTimelineAsync(driverId, current);
            timelines.Add(daily);
            current = current.AddDays(1);
        }

        return Result<List<DailyTimelineDto>>.Success(timelines);
    }

    public async Task<Result<DailyTimelineDto>> GetFilteredAsync(Guid driverId, DateOnly date, List<TimelineEventType> eventTypes)
    {
        _logger.LogInformation("Filtered timeline requested by driver {DriverId} for date {Date}", driverId, date);

        var timeline = await BuildDailyTimelineAsync(driverId, date);

        // Filter events by requested types
        timeline.Events = timeline.Events
            .Where(e => eventTypes.Contains(e.EventType))
            .ToList();

        // Recalculate summary based on filtered events
        timeline.Summary.TotalEvents = timeline.Events.Count;

        return Result<DailyTimelineDto>.Success(timeline);
    }

    private async Task<DailyTimelineDto> BuildDailyTimelineAsync(Guid driverId, DateOnly date)
    {
        var dayStart = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var dayEnd = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var events = new List<DriverTimelineEventDto>();

        // Get orders for the day
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var orderSpec = new TimelineOrdersSpec(driverId, dayStart, dayEnd);
        var orders = await orderRepo.ListAsync(orderSpec);

        foreach (var order in orders)
        {
            if (order.PickedUpAt.HasValue)
            {
                events.Add(new DriverTimelineEventDto
                {
                    EventType = TimelineEventType.OrderPickup,
                    Title = "استلام طلب",
                    Description = $"طلب #{order.OrderNumber} - {order.CustomerName ?? "عميل"}",
                    Timestamp = order.PickedUpAt.Value,
                    Amount = order.Amount,
                    OrderId = order.Id,
                    Icon = "package"
                });
            }

            if (order.DeliveredAt.HasValue)
            {
                events.Add(new DriverTimelineEventDto
                {
                    EventType = TimelineEventType.OrderDelivered,
                    Title = "تسليم طلب",
                    Description = $"طلب #{order.OrderNumber} - {order.DeliveryAddress}",
                    Timestamp = order.DeliveredAt.Value,
                    Amount = order.Amount,
                    OrderId = order.Id,
                    Icon = "check-circle"
                });
            }

            if (order.FailedAt.HasValue)
            {
                events.Add(new DriverTimelineEventDto
                {
                    EventType = TimelineEventType.OrderFailed,
                    Title = "طلب فاشل",
                    Description = $"طلب #{order.OrderNumber} - {order.ReturnReason ?? "فشل التسليم"}",
                    Timestamp = order.FailedAt.Value,
                    Amount = order.Amount,
                    OrderId = order.Id,
                    Icon = "x-circle"
                });
            }
        }

        // Get expenses for the day
        var expenseRepo = _unitOfWork.GetRepository<Expense, Guid>();
        var expenseSpec = new TimelineExpensesSpec(driverId, date);
        var expenses = await expenseRepo.ListAsync(expenseSpec);

        foreach (var expense in expenses)
        {
            events.Add(new DriverTimelineEventDto
            {
                EventType = TimelineEventType.Expense,
                Title = "مصروف",
                Description = $"{expense.ExpenseType} - {expense.Notes ?? "مصروف"}",
                Timestamp = expense.Date.ToDateTime(new TimeOnly(12, 0), DateTimeKind.Utc),
                Amount = expense.Amount,
                Icon = "credit-card"
            });
        }

        // Sort events by timestamp
        events = events.OrderBy(e => e.Timestamp).ToList();

        var deliveredOrders = orders.Where(o => o.Status == OrderStatus.Delivered).ToList();
        var totalEarnings = deliveredOrders.Sum(o => o.Amount);
        var totalExpenses = expenses.Sum(e => e.Amount);

        var timeline = new DailyTimelineDto
        {
            Date = date,
            Events = events,
            Summary = new TimelineSummaryDto
            {
                TotalEvents = events.Count,
                TotalOrders = orders.Count,
                TotalEarnings = totalEarnings,
                TotalExpenses = totalExpenses,
                NetProfit = totalEarnings - totalExpenses,
                TimeWorkedMinutes = 0 // Would require shift tracking
            }
        };

        return timeline;
    }
}

// ── Specifications ──

internal class TimelineOrdersSpec : BaseSpecification<Order>
{
    public TimelineOrdersSpec(Guid driverId, DateTime dayStart, DateTime dayEnd)
    {
        SetCriteria(o => o.DriverId == driverId && o.CreatedAt >= dayStart && o.CreatedAt <= dayEnd);
        SetOrderBy(o => o.CreatedAt);
    }
}

internal class TimelineExpensesSpec : BaseSpecification<Expense>
{
    public TimelineExpensesSpec(Guid driverId, DateOnly date)
    {
        SetCriteria(e => e.DriverId == driverId && e.Date == date);
    }
}
