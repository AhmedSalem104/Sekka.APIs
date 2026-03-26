using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Wallet;

public class WalletBalanceDto
{
    public decimal CashOnHand { get; set; }
    public decimal CashAlertThreshold { get; set; }
    public decimal CashAlertPercentage { get; set; }
    public decimal TodayCollected { get; set; }
    public decimal TodayCommissions { get; set; }
    public decimal PendingSettlements { get; set; }
}

public class WalletTransactionDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Description { get; set; }
    public string? OrderNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TransactionFilterDto : PaginationDto
{
    public TransactionType? TransactionType { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public class WalletSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetBalance { get; set; }
    public decimal CashOnHand { get; set; }
    public decimal PendingSettlements { get; set; }
}

public class CashStatusDto
{
    public decimal CashOnHand { get; set; }
    public decimal Threshold { get; set; }
    public decimal Percentage { get; set; }
    public string AlertLevel { get; set; } = null!;
    public string? NearestDepositPoint { get; set; }
    public string SuggestedAction { get; set; } = null!;
}
