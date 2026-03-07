namespace Sekka.Core.DTOs.Profile;

public class DriverStatsDto
{
    public int TotalOrders { get; set; }
    public int TotalDelivered { get; set; }
    public int TotalFailed { get; set; }
    public int TotalCancelled { get; set; }
    public decimal SuccessRate { get; set; }
    public decimal AverageRating { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalCommissions { get; set; }
    public decimal AverageDeliveryTimeMinutes { get; set; }
    public DateTime? BestDay { get; set; }
    public int BestDayOrders { get; set; }
}
