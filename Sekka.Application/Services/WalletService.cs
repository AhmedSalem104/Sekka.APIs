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

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WalletService> _logger;

    public WalletService(IUnitOfWork unitOfWork, ILogger<WalletService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<WalletBalanceDto>> GetBalanceAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<WalletTransaction, Guid>();
        var spec = new DriverWalletTransactionsSpec(driverId);
        var transactions = await repo.ListAsync(spec);

        var lastTx = transactions.OrderByDescending(t => t.CreatedAt).FirstOrDefault();

        var settlementRepo = _unitOfWork.GetRepository<Settlement, Guid>();
        var pendingSpec = new DriverSettlementsSpec(driverId);
        var settlements = await settlementRepo.ListAsync(pendingSpec);
        var pendingAmount = settlements.Sum(s => s.Amount);

        return Result<WalletBalanceDto>.Success(new WalletBalanceDto
        {
            DriverId = driverId,
            Balance = lastTx?.BalanceAfter ?? 0,
            CashOnHand = transactions.Where(t => t.TransactionType == TransactionType.OrderPayment).Sum(t => t.Amount),
            PendingSettlements = pendingAmount,
            LastUpdated = lastTx?.CreatedAt ?? DateTime.UtcNow
        });
    }

    public async Task<Result<List<WalletTransactionDto>>> GetTransactionsAsync(Guid driverId, WalletTransactionFilterDto filter)
    {
        var repo = _unitOfWork.GetRepository<WalletTransaction, Guid>();
        var spec = new DriverWalletTransactionsSpec(driverId, filter.TransactionType, filter.DateFrom, filter.DateTo);
        var transactions = await repo.ListAsync(spec);

        var dtos = transactions
            .OrderByDescending(t => t.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => new WalletTransactionDto
            {
                Id = t.Id,
                DriverId = t.DriverId,
                OrderId = t.OrderId,
                SettlementId = t.SettlementId,
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                BalanceAfter = t.BalanceAfter,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            }).ToList();

        return Result<List<WalletTransactionDto>>.Success(dtos);
    }

    public async Task<Result<WalletSummaryDto>> GetSummaryAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<WalletTransaction, Guid>();
        var spec = new DriverWalletTransactionsSpec(driverId);
        var transactions = await repo.ListAsync(spec);

        return Result<WalletSummaryDto>.Success(new WalletSummaryDto
        {
            TotalEarnings = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount),
            TotalExpenses = transactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => Math.Abs(t.Amount)),
            TotalSettlements = transactions.Where(t => t.TransactionType == TransactionType.Settlement).Sum(t => Math.Abs(t.Amount)),
            NetBalance = transactions.OrderByDescending(t => t.CreatedAt).FirstOrDefault()?.BalanceAfter ?? 0,
            TransactionCount = transactions.Count
        });
    }

    public async Task<Result<CashStatusDto>> GetCashStatusAsync(Guid driverId)
    {
        var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();
        var driver = await driverRepo.GetByIdAsync(driverId);
        if (driver == null)
            return Result<CashStatusDto>.NotFound(ErrorMessages.DriverNotFound);

        var settlementRepo = _unitOfWork.GetRepository<Settlement, Guid>();
        var spec = new DriverSettlementsSpec(driverId);
        var settlements = await settlementRepo.ListAsync(spec);
        var lastSettlement = settlements.OrderByDescending(s => s.SettledAt).FirstOrDefault();

        var hoursSince = lastSettlement != null
            ? (int)(DateTime.UtcNow - lastSettlement.SettledAt).TotalHours
            : 0;

        return Result<CashStatusDto>.Success(new CashStatusDto
        {
            DriverId = driverId,
            CashOnHand = driver.CashOnHand,
            CashAlertThreshold = driver.CashAlertThreshold,
            IsOverThreshold = driver.CashOnHand >= driver.CashAlertThreshold,
            LastSettlementAt = lastSettlement?.SettledAt,
            HoursSinceLastSettlement = hoursSince
        });
    }
}

internal class DriverWalletTransactionsSpec : BaseSpecification<WalletTransaction>
{
    public DriverWalletTransactionsSpec(Guid driverId, TransactionType? type = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        SetCriteria(t => t.DriverId == driverId
            && (!type.HasValue || t.TransactionType == type.Value)
            && (!dateFrom.HasValue || t.CreatedAt >= dateFrom.Value)
            && (!dateTo.HasValue || t.CreatedAt <= dateTo.Value));
        SetOrderByDescending(t => t.CreatedAt);
    }
}

internal class DriverSettlementsSpec : BaseSpecification<Settlement>
{
    public DriverSettlementsSpec(Guid driverId)
    {
        SetCriteria(s => s.DriverId == driverId);
        SetOrderByDescending(s => s.SettledAt);
    }
}
