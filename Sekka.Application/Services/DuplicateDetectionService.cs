using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class DuplicateDetectionService : IDuplicateDetectionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DuplicateDetectionService> _logger;

    public DuplicateDetectionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DuplicateDetectionService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<DuplicateResultDto>> CheckDuplicateAsync(Guid driverId, CheckDuplicateDto dto)
    {
        _logger.LogInformation("Duplicate check requested by driver {DriverId}", driverId);

        var normalizedPhone = EgyptianPhoneHelper.Normalize(dto.CustomerPhone);
        var cutoff = DateTime.UtcNow.AddHours(-24);

        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var spec = new RecentOrdersByPhoneSpec(driverId, normalizedPhone, cutoff);
        var recentOrders = await orderRepo.ListAsync(spec);

        if (recentOrders.Count == 0)
        {
            return Result<DuplicateResultDto>.Success(new DuplicateResultDto
            {
                IsDuplicate = false,
                MatchScore = 0,
                SuggestedAction = DuplicateAction.AllowCreate
            });
        }

        // Calculate match score based on phone + address similarity
        var bestMatch = recentOrders
            .Select(o => new
            {
                Order = o,
                Score = CalculateMatchScore(o, dto)
            })
            .OrderByDescending(x => x.Score)
            .First();

        var isDuplicate = bestMatch.Score >= 0.7m;

        var result = new DuplicateResultDto
        {
            IsDuplicate = isDuplicate,
            MatchScore = bestMatch.Score,
            MatchedOrder = _mapper.Map<OrderListDto>(bestMatch.Order),
            SuggestedAction = bestMatch.Score >= 0.9m ? DuplicateAction.Block
                : bestMatch.Score >= 0.7m ? DuplicateAction.Warn
                : DuplicateAction.AllowCreate
        };

        return Result<DuplicateResultDto>.Success(result);
    }

    private static decimal CalculateMatchScore(Order order, CheckDuplicateDto dto)
    {
        decimal score = 0;

        // Phone match (already filtered by phone, so this is guaranteed) = 0.5
        score += 0.5m;

        // Address similarity
        if (!string.IsNullOrEmpty(order.DeliveryAddress) && !string.IsNullOrEmpty(dto.DeliveryAddress))
        {
            var addressNorm1 = order.DeliveryAddress.Trim().ToLowerInvariant();
            var addressNorm2 = dto.DeliveryAddress.Trim().ToLowerInvariant();

            if (addressNorm1 == addressNorm2)
                score += 0.4m;
            else if (addressNorm1.Contains(addressNorm2) || addressNorm2.Contains(addressNorm1))
                score += 0.25m;
        }

        // Amount match
        if (dto.Amount.HasValue && order.Amount == dto.Amount.Value)
            score += 0.1m;

        return Math.Min(score, 1.0m);
    }
}

// ── Specifications ──

internal class RecentOrdersByPhoneSpec : BaseSpecification<Order>
{
    public RecentOrdersByPhoneSpec(Guid driverId, string phone, DateTime cutoff)
    {
        SetCriteria(o => o.DriverId == driverId
            && o.CustomerPhone == phone
            && o.CreatedAt >= cutoff);
        SetOrderByDescending(o => o.CreatedAt);
    }
}
