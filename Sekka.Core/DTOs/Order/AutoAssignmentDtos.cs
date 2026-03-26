using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Order;

public class SuggestedDriverDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public string DriverPhone { get; set; } = null!;
    public double DistanceKm { get; set; }
    public int CurrentOrders { get; set; }
    public decimal Rating { get; set; }
    public decimal Score { get; set; }
    public bool IsOnline { get; set; }
    public VehicleType VehicleType { get; set; }
}

public class AssignmentConfigDto
{
    public AssignmentStrategy Strategy { get; set; } = AssignmentStrategy.Balanced;
    public double MaxDistanceKm { get; set; } = 10;
    public int MaxCurrentOrders { get; set; } = 5;
    public decimal MinRating { get; set; } = 3.0m;
}
