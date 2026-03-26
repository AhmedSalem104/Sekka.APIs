using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Order;

public class TrackingPageDto
{
    public string OrderNumber { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public string? DriverName { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public string? PickupAddress { get; set; }
    public string DeliveryAddress { get; set; } = null!;
    public double? DriverLatitude { get; set; }
    public double? DriverLongitude { get; set; }
    public List<TrackingTimelineDto> Timeline { get; set; } = new();
    public string? PartnerName { get; set; }
    public string? PartnerLogoUrl { get; set; }
}

public class TrackingTimelineDto
{
    public OrderStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
    public string Description { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public bool IsCurrent { get; set; }
}
