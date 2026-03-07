using Sekka.Core.DTOs.Common;
namespace Sekka.Core.DTOs.Admin;
public class AdminDriverFilterDto : PaginationDto
{
    public bool? IsActive { get; set; }
    public bool? IsOnline { get; set; }
    public string? SearchTerm { get; set; }
    public Guid? RegionId { get; set; }
}
public class AdminDriverDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool IsOnline { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageRating { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class AdminDriverDetailDto : AdminDriverDto
{
    public string? Email { get; set; }
    public string? ProfileImageUrl { get; set; }
    public decimal CashOnHand { get; set; }
    public string? SubscriptionPlan { get; set; }
    public DateTime? LastLocationUpdate { get; set; }
}
public class DriverPerformanceDto
{
    public int TotalOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public decimal SuccessRate { get; set; }
    public decimal AverageRating { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal AverageDeliveryTime { get; set; }
}
public class DriverLocationDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastUpdate { get; set; }
}
