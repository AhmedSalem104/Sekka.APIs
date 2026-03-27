using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Social;

public class StartShiftDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class ShiftDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public ShiftStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public double? EndLatitude { get; set; }
    public double? EndLongitude { get; set; }
    public int OrdersCompleted { get; set; }
    public decimal EarningsTotal { get; set; }
    public double DistanceKm { get; set; }
}

public class ShiftSummaryDto
{
    public int TotalShifts { get; set; }
    public double TotalHoursWorked { get; set; }
    public int TotalOrdersCompleted { get; set; }
    public decimal TotalEarnings { get; set; }
    public double TotalDistanceKm { get; set; }
    public double AverageShiftDurationHours { get; set; }
}
