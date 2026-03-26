namespace Sekka.Core.DTOs.Order;

public class TimeSlotDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentBookings { get; set; }
    public int AvailableSlots { get; set; }
    public decimal PriceMultiplier { get; set; }
    public bool IsAvailable { get; set; }
}

public class AvailableSlotsDto
{
    public DateOnly Date { get; set; }
    public List<TimeSlotDto> Slots { get; set; } = new();
}

public class BookSlotDto
{
    public Guid TimeSlotId { get; set; }
}

public class CreateTimeSlotDto
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int MaxCapacity { get; set; }
    public Guid? RegionId { get; set; }
    public decimal PriceMultiplier { get; set; } = 1.0m;
}

public class UpdateTimeSlotDto
{
    public int? MaxCapacity { get; set; }
    public decimal? PriceMultiplier { get; set; }
    public bool? IsActive { get; set; }
}

public class GenerateWeekSlotsDto
{
    public DateOnly StartDate { get; set; }
    public TimeOnly DayStart { get; set; } = new TimeOnly(8, 0);
    public TimeOnly DayEnd { get; set; } = new TimeOnly(22, 0);
    public int SlotDurationMinutes { get; set; } = 120;
    public int MaxCapacityPerSlot { get; set; } = 10;
    public Guid? RegionId { get; set; }
}

public class TimeSlotStatsDto
{
    public int TotalSlots { get; set; }
    public int TotalBookings { get; set; }
    public decimal UtilizationRate { get; set; }
    public TimeOnly PeakHour { get; set; }
}
