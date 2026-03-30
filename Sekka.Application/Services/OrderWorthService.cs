using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class OrderWorthService : IOrderWorthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderWorthService> _logger;

    // Egypt fuel cost constants (EGP)
    private const decimal FuelCostPerKm = 2.5m;
    private const decimal BasePricePerKm = 8.0m;
    private const decimal MinimumPrice = 20.0m;
    private const double AverageSpeedKmPerHour = 30.0;

    public OrderWorthService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderWorthService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<OrderWorthDto>> CalculateWorthAsync(Guid driverId, Guid orderId)
    {
        _logger.LogInformation("Worth calculation requested for order {OrderId} by driver {DriverId}", orderId, driverId);

        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await orderRepo.GetByIdAsync(orderId);

        if (order is null || order.DriverId != driverId)
            return Result<OrderWorthDto>.NotFound(ErrorMessages.ItemNotFound);

        var distanceKm = order.DistanceKm ?? 5.0; // default estimate
        var fuelCost = (decimal)distanceKm * FuelCostPerKm;
        var estimatedProfit = order.Amount - fuelCost;
        var estimatedMinutes = (int)(distanceKm / AverageSpeedKmPerHour * 60);

        // Worth score: 0-100
        var worthScore = CalculateWorthScore(order.Amount, fuelCost, distanceKm, estimatedMinutes);

        var factors = new List<WorthFactorDto>
        {
            new() { FactorName = "المبلغ", Value = $"{order.Amount:F2} ج.م", Impact = order.Amount >= 100 ? "إيجابي" : "محايد" },
            new() { FactorName = "المسافة", Value = $"{distanceKm:F1} كم", Impact = distanceKm <= 5 ? "إيجابي" : "سلبي" },
            new() { FactorName = "تكلفة الوقود", Value = $"{fuelCost:F2} ج.م", Impact = fuelCost < order.Amount * 0.3m ? "إيجابي" : "سلبي" },
            new() { FactorName = "الوقت المتوقع", Value = $"{estimatedMinutes} دقيقة", Impact = estimatedMinutes <= 30 ? "إيجابي" : "سلبي" }
        };

        var recommendation = worthScore >= 70 ? "طلب مربح - يُنصح بقبوله"
            : worthScore >= 40 ? "طلب متوسط - قرارك"
            : "طلب منخفض القيمة - فكّر مرتين";

        var dto = new OrderWorthDto
        {
            WorthScore = worthScore,
            EstimatedProfit = estimatedProfit,
            EstimatedFuelCost = fuelCost,
            EstimatedTimeMinutes = estimatedMinutes,
            DistanceKm = distanceKm,
            Recommendation = recommendation,
            Factors = factors
        };

        return Result<OrderWorthDto>.Success(dto);
    }

    public async Task<Result<PriceCalculationResultDto>> CalculatePriceAsync(PriceCalculationRequestDto dto)
    {
        _logger.LogInformation("Price calculation requested");

        var distanceKm = HaversineDistance(
            dto.PickupLatitude, dto.PickupLongitude,
            dto.DeliveryLatitude, dto.DeliveryLongitude);

        var estimatedMinutes = (int)(distanceKm / AverageSpeedKmPerHour * 60);
        var basePrice = Math.Max((decimal)distanceKm * BasePricePerKm, MinimumPrice);
        var fuelCost = (decimal)distanceKm * FuelCostPerKm;

        // Priority multiplier
        var priorityMultiplier = dto.Priority switch
        {
            OrderPriority.Urgent => 1.5m,
            OrderPriority.VIP => 1.25m,
            _ => 1.0m
        };

        // Peak hour surcharge (7-10 AM, 5-8 PM)
        var hour = DateTime.UtcNow.AddHours(2).Hour; // Egypt is UTC+2
        var peakSurcharge = (hour >= 7 && hour <= 10) || (hour >= 17 && hour <= 20)
            ? basePrice * 0.15m
            : 0m;

        // Item count surcharge
        var itemSurcharge = dto.ItemCount > 1 ? (dto.ItemCount - 1) * 5m : 0m;

        var suggestedPrice = (basePrice * priorityMultiplier) + peakSurcharge + itemSurcharge;
        var minPrice = suggestedPrice * 0.8m;
        var maxPrice = suggestedPrice * 1.3m;

        var breakdown = new List<PriceBreakdownItem>
        {
            new() { Label = "سعر أساسي", Amount = basePrice, Percentage = basePrice / suggestedPrice * 100 },
            new() { Label = "تكلفة الوقود", Amount = fuelCost, Percentage = fuelCost / suggestedPrice * 100 }
        };

        if (priorityMultiplier > 1.0m)
            breakdown.Add(new PriceBreakdownItem { Label = "رسوم الأولوية", Amount = basePrice * (priorityMultiplier - 1), Percentage = (priorityMultiplier - 1) * 100 });

        if (peakSurcharge > 0)
            breakdown.Add(new PriceBreakdownItem { Label = "رسوم ساعة الذروة", Amount = peakSurcharge, Percentage = peakSurcharge / suggestedPrice * 100 });

        if (itemSurcharge > 0)
            breakdown.Add(new PriceBreakdownItem { Label = "رسوم قطع إضافية", Amount = itemSurcharge, Percentage = itemSurcharge / suggestedPrice * 100 });

        var result = new PriceCalculationResultDto
        {
            EstimatedDistanceKm = distanceKm,
            EstimatedTimeMinutes = estimatedMinutes,
            SuggestedPrice = Math.Round(suggestedPrice, 2),
            MinPrice = Math.Round(minPrice, 2),
            MaxPrice = Math.Round(maxPrice, 2),
            FuelCostEstimate = Math.Round(fuelCost, 2),
            PriorityMultiplier = priorityMultiplier,
            PeakHourSurcharge = Math.Round(peakSurcharge, 2),
            Breakdown = breakdown
        };

        return await Task.FromResult(Result<PriceCalculationResultDto>.Success(result));
    }

    private static decimal CalculateWorthScore(decimal amount, decimal fuelCost, double distanceKm, int timeMinutes)
    {
        var profitRatio = amount > 0 ? (amount - fuelCost) / amount * 100 : 0;
        var distanceScore = distanceKm <= 3 ? 30m : distanceKm <= 10 ? 20m : 10m;
        var timeScore = timeMinutes <= 15 ? 20m : timeMinutes <= 30 ? 15m : 5m;
        var amountScore = amount >= 200 ? 30m : amount >= 100 ? 20m : amount >= 50 ? 15m : 5m;

        return Math.Min(Math.Max(distanceScore + timeScore + amountScore + (profitRatio > 50 ? 20 : 10), 0), 100);
    }

    /// <summary>
    /// Haversine formula to calculate distance between two coordinates in km.
    /// </summary>
    private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0; // Earth's radius in km

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
            + Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2))
            * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return Math.Round(R * c, 2);
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
}
