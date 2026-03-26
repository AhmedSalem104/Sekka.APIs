using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Order;

public class UpdateStatusDto
{
    public OrderStatus Status { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class DeliverOrderDto
{
    public decimal? ActualCollectedAmount { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
    public int? RatingValue { get; set; }
}

public class FailOrderDto
{
    public DeliveryFailReason Reason { get; set; }
    public string? ReasonText { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool SendAutoMessage { get; set; } = true;
}

public class CancelOrderDto
{
    public CancellationReason CancellationReason { get; set; }
    public string? ReasonText { get; set; }
    public decimal LossAmount { get; set; }
    public double? DistanceTravelledKm { get; set; }
    public decimal? FuelCostLost { get; set; }
    public Guid? TransferToDriverId { get; set; }
}

public class TransferOrderDto
{
    public Guid? ToDriverId { get; set; }
    public string? Reason { get; set; }
}

public class PartialDeliveryDto
{
    public int DeliveredItemCount { get; set; }
    public int ReturnedItemCount { get; set; }
    public decimal ActualAmount { get; set; }
    public string? ReturnReason { get; set; }
    public string? Notes { get; set; }
}
