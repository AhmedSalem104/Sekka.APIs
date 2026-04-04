using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class SettlementService : ISettlementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SettlementService> _logger;

    public SettlementService(IUnitOfWork unitOfWork, ILogger<SettlementService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<SettlementDto>>> GetSettlementsAsync(Guid driverId, SettlementFilterDto filter)
    {
        var repo = _unitOfWork.GetRepository<Settlement, Guid>();
        var spec = new SettlementsByDriverSpec(driverId, filter.PartnerId, filter.SettlementType, filter.DateFrom, filter.DateTo);
        var settlements = await repo.ListAsync(spec);

        // Load partner names
        var partnerRepo = _unitOfWork.GetRepository<Partner, Guid>();
        var partnerIds = settlements.Select(s => s.PartnerId).Distinct().ToList();
        var partners = new Dictionary<Guid, string>();
        foreach (var pid in partnerIds)
        {
            var p = await partnerRepo.GetByIdAsync(pid);
            if (p != null) partners[pid] = p.Name;
        }

        var dtos = settlements
            .OrderByDescending(s => s.SettledAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(s =>
            {
                var dto = MapToDto(s);
                dto.PartnerName = partners.GetValueOrDefault(s.PartnerId);
                return dto;
            })
            .ToList();

        return Result<List<SettlementDto>>.Success(dtos);
    }

    public async Task<Result<SettlementDto>> CreateAsync(Guid driverId, CreateSettlementDto dto)
    {
        var partnerRepo = _unitOfWork.GetRepository<Partner, Guid>();
        var partner = await partnerRepo.GetByIdAsync(dto.PartnerId);
        if (partner == null)
            return Result<SettlementDto>.NotFound(ErrorMessages.PartnerNotFound);

        var settlement = new Settlement
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            PartnerId = dto.PartnerId,
            Amount = dto.Amount,
            SettlementType = dto.SettlementType,
            OrderCount = dto.OrderCount,
            Notes = dto.Notes,
            SettledAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        var repo = _unitOfWork.GetRepository<Settlement, Guid>();
        await repo.AddAsync(settlement);

        // Create wallet transaction for the settlement
        var txRepo = _unitOfWork.GetRepository<WalletTransaction, Guid>();
        var walletSpec = new LatestWalletTransactionSpec(driverId);
        var latestTx = (await txRepo.ListAsync(walletSpec)).FirstOrDefault();
        var currentBalance = latestTx?.BalanceAfter ?? 0;

        var walletTx = new WalletTransaction
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            SettlementId = settlement.Id,
            Amount = -dto.Amount,
            TransactionType = TransactionType.Settlement,
            BalanceAfter = currentBalance - dto.Amount,
            Description = $"تسوية مع {partner.Name}",
            CreatedAt = DateTime.UtcNow
        };
        await txRepo.AddAsync(walletTx);

        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Settlement {SettlementId} created for driver {DriverId}, partner {PartnerId}, amount {Amount}",
            settlement.Id, driverId, dto.PartnerId, dto.Amount);

        var resultDto = MapToDto(settlement);
        resultDto.PartnerName = partner.Name;
        return Result<SettlementDto>.Success(resultDto);
    }

    public async Task<Result<PartnerBalanceDto>> GetPartnerBalanceAsync(Guid driverId, Guid partnerId)
    {
        var partnerRepo = _unitOfWork.GetRepository<Partner, Guid>();
        var partner = await partnerRepo.GetByIdAsync(partnerId);
        if (partner == null)
            return Result<PartnerBalanceDto>.NotFound(ErrorMessages.PartnerNotFound);

        var repo = _unitOfWork.GetRepository<Settlement, Guid>();
        var spec = new SettlementsByDriverSpec(driverId, partnerId);
        var settlements = await repo.ListAsync(spec);

        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var orderSpec = new DeliveredOrdersByPartnerSpec(driverId, partnerId);
        var deliveredOrders = await orderRepo.ListAsync(orderSpec);

        var totalCollected = deliveredOrders.Sum(o => o.Amount);
        var totalSettled = settlements.Sum(s => s.Amount);

        return Result<PartnerBalanceDto>.Success(new PartnerBalanceDto
        {
            PartnerId = partnerId,
            PartnerName = partner.Name,
            TotalCollected = totalCollected,
            TotalSettled = totalSettled,
            PendingBalance = totalCollected - totalSettled,
            PendingOrderCount = deliveredOrders.Count
        });
    }

    public async Task<Result<DailySettlementSummaryDto>> GetDailySummaryAsync(Guid driverId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var repo = _unitOfWork.GetRepository<Settlement, Guid>();
        var spec = new SettlementsByDriverSpec(driverId, dateFrom: DateTime.UtcNow.Date, dateTo: DateTime.UtcNow.Date.AddDays(1));
        var settlements = await repo.ListAsync(spec);

        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);
        var orderSpec = new DeliveredOrdersByDriverDateSpec(driverId, todayStart, todayEnd);
        var deliveredOrders = await orderRepo.ListAsync(orderSpec);

        var totalCollected = deliveredOrders.Sum(o => o.Amount);
        var totalSettled = settlements.Sum(s => s.Amount);

        var partnerIds = deliveredOrders
            .Where(o => o.PartnerId.HasValue)
            .Select(o => o.PartnerId!.Value)
            .Distinct();
        var settledPartnerIds = settlements.Select(s => s.PartnerId).Distinct();
        var pendingPartners = partnerIds.Count(pid => !settledPartnerIds.Contains(pid));

        return Result<DailySettlementSummaryDto>.Success(new DailySettlementSummaryDto
        {
            Date = today,
            TotalCollected = totalCollected,
            TotalSettled = totalSettled,
            RemainingBalance = totalCollected - totalSettled,
            SettlementCount = settlements.Count,
            PendingPartners = pendingPartners
        });
    }

    public Task<Result<bool>> UploadReceiptAsync(Guid driverId, Guid id, Stream stream, string fileName)
    {
        // File upload would be handled by a storage service
        _logger.LogInformation("Receipt upload requested for settlement {Id} by driver {DriverId}, file: {FileName}",
            id, driverId, fileName);
        return Task.FromResult(Result<bool>.Success(true));
    }

    private static SettlementDto MapToDto(Settlement s) => new()
    {
        Id = s.Id,
        DriverId = s.DriverId,
        PartnerId = s.PartnerId,
        Amount = s.Amount,
        SettlementType = s.SettlementType,
        OrderCount = s.OrderCount,
        Notes = s.Notes,
        ReceiptImageUrl = s.ReceiptImageUrl,
        WhatsAppSent = s.WhatsAppSent,
        SettledAt = s.SettledAt,
        CreatedAt = s.CreatedAt
    };
}

internal class SettlementsByDriverSpec : BaseSpecification<Settlement>
{
    public SettlementsByDriverSpec(Guid driverId, Guid? partnerId = null, SettlementType? type = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        SetCriteria(s => s.DriverId == driverId
            && (!partnerId.HasValue || s.PartnerId == partnerId.Value)
            && (!type.HasValue || s.SettlementType == type.Value)
            && (!dateFrom.HasValue || s.SettledAt >= dateFrom.Value)
            && (!dateTo.HasValue || s.SettledAt <= dateTo.Value));
        SetOrderByDescending(s => s.SettledAt);
    }
}

internal class DeliveredOrdersByDriverDateSpec : BaseSpecification<Order>
{
    public DeliveredOrdersByDriverDateSpec(Guid driverId, DateTime from, DateTime to)
    {
        SetCriteria(o => o.DriverId == driverId
            && o.Status == OrderStatus.Delivered
            && o.CreatedAt >= from && o.CreatedAt < to);
    }
}

internal class DeliveredOrdersByPartnerSpec : BaseSpecification<Order>
{
    public DeliveredOrdersByPartnerSpec(Guid driverId, Guid partnerId)
    {
        SetCriteria(o => o.DriverId == driverId
            && o.PartnerId == partnerId
            && o.Status == OrderStatus.Delivered);
    }
}

internal class LatestWalletTransactionSpec : BaseSpecification<WalletTransaction>
{
    public LatestWalletTransactionSpec(Guid driverId)
    {
        SetCriteria(t => t.DriverId == driverId);
        SetOrderByDescending(t => t.CreatedAt);
        ApplyPaging(0, 1);
    }
}
