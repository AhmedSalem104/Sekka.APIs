using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Intelligence;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class BehaviorAnalysisService : IBehaviorAnalysisService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BehaviorAnalysisService> _logger;

    public BehaviorAnalysisService(IUnitOfWork unitOfWork, ILogger<BehaviorAnalysisService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CustomerBehaviorSummaryDto>> GetBehaviorSummaryAsync(Guid driverId, Guid customerId)
    {
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var spec = new CustomerOrdersSpec(driverId, customerId);
        var orders = await orderRepo.ListAsync(spec);

        if (!orders.Any())
        {
            return Result<CustomerBehaviorSummaryDto>.Success(new CustomerBehaviorSummaryDto
            {
                AverageOrderValue = 0,
                OrderFrequencyPerMonth = 0,
                SpendingTier = "New",
                PreferredAreas = new List<string>(),
                Patterns = new List<BehaviorPatternDto>()
            });
        }

        var deliveredOrders = orders.Where(o => o.Status == Core.Enums.OrderStatus.Delivered).ToList();
        var avgValue = deliveredOrders.Any() ? deliveredOrders.Average(o => o.Amount) : 0;
        var months = Math.Max(1, (DateTime.UtcNow - orders.Min(o => o.CreatedAt)).Days / 30.0);
        var frequency = Math.Round((decimal)(orders.Count / months), 1);

        // Determine preferred time
        var hourGroups = deliveredOrders.GroupBy(o => o.CreatedAt.Hour / 6).OrderByDescending(g => g.Count()).FirstOrDefault();
        var preferredTime = hourGroups?.Key switch
        {
            0 => "night",
            1 => "morning",
            2 => "afternoon",
            3 => "evening",
            _ => null
        };

        // Determine preferred day
        var dayGroups = deliveredOrders.GroupBy(o => o.CreatedAt.DayOfWeek).OrderByDescending(g => g.Count()).FirstOrDefault();

        // Spending tier
        var spendingTier = avgValue switch
        {
            < 50 => "Low",
            < 150 => "Medium",
            < 500 => "High",
            _ => "Premium"
        };

        // Top areas
        var areas = deliveredOrders
            .Where(o => !string.IsNullOrEmpty(o.DeliveryAddress))
            .GroupBy(o => o.DeliveryAddress.Split(',')[0].Trim())
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => g.Key)
            .ToList();

        return Result<CustomerBehaviorSummaryDto>.Success(new CustomerBehaviorSummaryDto
        {
            PreferredOrderTime = preferredTime,
            PreferredDayOfWeek = dayGroups?.Key.ToString(),
            AverageOrderValue = Math.Round(avgValue, 2),
            OrderFrequencyPerMonth = frequency,
            SpendingTier = spendingTier,
            PreferredAreas = areas,
            Patterns = new List<BehaviorPatternDto>()
        });
    }
}

internal class CustomerOrdersSpec : BaseSpecification<Order>
{
    public CustomerOrdersSpec(Guid driverId, Guid customerId)
    {
        SetCriteria(o => o.DriverId == driverId && o.CustomerId == customerId);
        SetOrderByDescending(o => o.CreatedAt);
    }
}
