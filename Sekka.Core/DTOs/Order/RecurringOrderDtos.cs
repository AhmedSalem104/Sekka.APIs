namespace Sekka.Core.DTOs.Order;

public class CreateRecurringOrderDto : CreateOrderDto
{
    public new string RecurrencePattern { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class UpdateRecurringOrderDto
{
    public string? RecurrencePattern { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class RecurringOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public string? CustomerName { get; set; }
    public string DeliveryAddress { get; set; } = null!;
    public decimal Amount { get; set; }
    public string RecurrencePattern { get; set; } = null!;
    public DateOnly? NextScheduledDate { get; set; }
    public int TotalOccurrences { get; set; }
    public bool IsPaused { get; set; }
}
