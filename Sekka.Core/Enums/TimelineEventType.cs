namespace Sekka.Core.Enums;

public enum TimelineEventType
{
    ShiftStart = 0,
    OrderPickup = 1,
    OrderDelivered = 2,
    OrderFailed = 3,
    Expense = 4,
    Settlement = 5,
    BreakStart = 6,
    BreakEnd = 7,
    ShiftEnd = 8
}
