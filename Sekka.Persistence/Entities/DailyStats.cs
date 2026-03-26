using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class DailyStats : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public DateOnly Date { get; set; }
    public int TotalOrders { get; set; }
    public int SuccessfulOrders { get; set; }
    public int FailedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public int PostponedOrders { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalCommissions { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public double TotalDistanceKm { get; set; }
    public int TimeWorkedMinutes { get; set; }
    public decimal CashCollected { get; set; }
    public int? AverageDeliveryTimeMinutes { get; set; }
    public string? BestRegion { get; set; }
    public string? BestTimeSlot { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
}
