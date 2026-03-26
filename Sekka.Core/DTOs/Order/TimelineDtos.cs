using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Order;

public class DailyTimelineDto
{
    public DateOnly Date { get; set; }
    public List<DriverTimelineEventDto> Events { get; set; } = new();
    public TimelineSummaryDto Summary { get; set; } = new();
}

public class DriverTimelineEventDto
{
    public TimelineEventType EventType { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public decimal? Amount { get; set; }
    public Guid? OrderId { get; set; }
    public string Icon { get; set; } = null!;
    public Dictionary<string, string>? Metadata { get; set; }
}

public class TimelineSummaryDto
{
    public int TotalEvents { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public int TimeWorkedMinutes { get; set; }
}
