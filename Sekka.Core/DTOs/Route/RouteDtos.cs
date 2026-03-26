using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Route;

public class OptimizeRouteDto
{
    public List<Guid> OrderIds { get; set; } = new();
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public string OptimizeFor { get; set; } = "time";
}

public class ReorderRouteDto
{
    public List<Guid> OrderIds { get; set; } = new();
}

public class AddOrderToRouteDto
{
    public Guid OrderId { get; set; }
    public int? InsertAtIndex { get; set; }
}

public class RouteDto
{
    public Guid Id { get; set; }
    public List<RouteOrderDto> Orders { get; set; } = new();
    public int? EstimatedTimeMinutes { get; set; }
    public double? TotalDistanceKm { get; set; }
    public decimal? EfficiencyScore { get; set; }
    public bool IsActive { get; set; }
}

public class RouteOrderDto
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public int SequenceIndex { get; set; }
    public string? CustomerName { get; set; }
    public string DeliveryAddress { get; set; } = null!;
    public decimal Amount { get; set; }
    public OrderStatus Status { get; set; }
    public int? EstimatedArrivalMinutes { get; set; }
}
