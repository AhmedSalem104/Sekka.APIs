using System.ComponentModel.DataAnnotations;

namespace Sekka.Core.DTOs.Social;

public class NearbyDriverDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double DistanceKm { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsOnline { get; set; }
    public string? VehicleType { get; set; }
}

public class CreateHelpRequestDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string HelpType { get; set; } = null!;
    public Guid? OrderId { get; set; }
    public string? DriverPhone { get; set; }
}

public class HelpRequestDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string HelpType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public Guid? ResponderId { get; set; }
    public string? ResponderName { get; set; }
    public string? ResponderPhone { get; set; }
    public string? DriverPhone { get; set; }
    public double? DistanceKm { get; set; }
    public Guid? OrderId { get; set; }
    public OrderSummaryDto? OrderSummary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class OrderSummaryDto
{
    public string OrderNumber { get; set; } = null!;
    public string? CustomerName { get; set; }
    public string DeliveryAddress { get; set; } = null!;
    public decimal Amount { get; set; }
    public int PaymentMethod { get; set; }
    public string? CustomerPhone { get; set; }
}

public class UpdateLocationDto
{
    [Range(-90, 90, ErrorMessage = "خط العرض لازم يكون بين -90 و 90")]
    public double Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "خط الطول لازم يكون بين -180 و 180")]
    public double Longitude { get; set; }
}
