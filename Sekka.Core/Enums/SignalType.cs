namespace Sekka.Core.Enums;

public enum SignalType
{
    OrderCreated = 0,
    OrderDelivered = 1,
    OrderReordered = 2,
    HighRating = 3,
    LowRating = 4,
    FrequentAddress = 5,
    HighValue = 6,
    Cancellation = 7,
    ReturnOrder = 8,
    RecurringOrder = 9,
    PartnerOrder = 10,
    TimeSlotBooked = 11
}
