using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Order;

public class OrderWorthDto
{
    public decimal WorthScore { get; set; }
    public decimal EstimatedProfit { get; set; }
    public decimal EstimatedFuelCost { get; set; }
    public int EstimatedTimeMinutes { get; set; }
    public double DistanceKm { get; set; }
    public string Recommendation { get; set; } = null!;
    public List<WorthFactorDto> Factors { get; set; } = new();
}

public class WorthFactorDto
{
    public string FactorName { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Impact { get; set; } = null!;
}

public class PriceCalculationRequestDto
{
    public double PickupLatitude { get; set; }
    public double PickupLongitude { get; set; }
    public double DeliveryLatitude { get; set; }
    public double DeliveryLongitude { get; set; }
    public int ItemCount { get; set; } = 1;
    public OrderPriority Priority { get; set; }
    public DateTime? TimeWindowStart { get; set; }
    public DateTime? TimeWindowEnd { get; set; }
}

public class PriceCalculationResultDto
{
    public double EstimatedDistanceKm { get; set; }
    public int EstimatedTimeMinutes { get; set; }
    public decimal SuggestedPrice { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal FuelCostEstimate { get; set; }
    public decimal PriorityMultiplier { get; set; }
    public decimal PeakHourSurcharge { get; set; }
    public List<PriceBreakdownItem> Breakdown { get; set; } = new();
}

public class PriceBreakdownItem
{
    public string Label { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}
