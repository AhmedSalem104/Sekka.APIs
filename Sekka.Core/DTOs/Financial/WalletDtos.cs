using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Financial;

public class WalletBalanceDto
{
    public Guid DriverId { get; set; }
    public decimal Balance { get; set; }
    public decimal CashOnHand { get; set; }
    public decimal PendingSettlements { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class WalletTransactionDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? SettlementId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WalletTransactionFilterDto : PaginationDto
{
    public TransactionType? TransactionType { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class WalletSummaryDto
{
    public decimal TotalEarnings { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalSettlements { get; set; }
    public decimal NetBalance { get; set; }
    public int TransactionCount { get; set; }
}

public class CashStatusDto
{
    public Guid DriverId { get; set; }
    public decimal CashOnHand { get; set; }
    public decimal CashAlertThreshold { get; set; }
    public bool IsOverThreshold { get; set; }
    public DateTime? LastSettlementAt { get; set; }
    public int HoursSinceLastSettlement { get; set; }
}

public class WalletAdjustmentDto
{
    public Guid DriverId { get; set; }
    public decimal Amount { get; set; }
    public WalletAdjustmentType AdjustmentType { get; set; }
    public string Reason { get; set; } = null!;
}

public class AdminWalletFilterDto : PaginationDto
{
    public string? Search { get; set; }
    public decimal? MinBalance { get; set; }
    public decimal? MaxBalance { get; set; }
    public bool? OverThreshold { get; set; }
}

public class AdminWalletSummaryDto
{
    public decimal TotalCashOnHand { get; set; }
    public decimal TotalPendingSettlements { get; set; }
    public int DriversOverThreshold { get; set; }
    public int ActiveWallets { get; set; }
}

public class WalletFreezeDto
{
    public Guid DriverId { get; set; }
    public WalletFreezeReason Reason { get; set; }
    public string? Notes { get; set; }
}
