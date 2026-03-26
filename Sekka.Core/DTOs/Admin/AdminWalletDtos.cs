using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Admin;

public class AdminWalletDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public string DriverPhone { get; set; } = null!;
    public decimal CurrentBalance { get; set; }
    public decimal CashOnHand { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal TotalDebits { get; set; }
    public bool IsFrozen { get; set; }
    public string? FreezeReason { get; set; }
    public DateTime? LastTransactionAt { get; set; }
}

public class WalletAdjustmentDto
{
    public decimal Amount { get; set; }
    public WalletAdjustmentType AdjustmentType { get; set; }
    public string Reason { get; set; } = null!;
    public string? Notes { get; set; }
}

public class FreezeWalletDto
{
    public WalletFreezeReason Reason { get; set; }
    public string Notes { get; set; } = null!;
}

public class BulkWalletAdjustmentDto
{
    public List<Guid> DriverIds { get; set; } = new();
    public decimal Amount { get; set; }
    public WalletAdjustmentType AdjustmentType { get; set; }
    public string Reason { get; set; } = null!;
}

public class BulkAdjustmentResultDto
{
    public int TotalDrivers { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public decimal TotalAmount { get; set; }
    public List<Guid> FailedDriverIds { get; set; } = new();
}

public class WalletStatsDto
{
    public int TotalWallets { get; set; }
    public decimal TotalBalance { get; set; }
    public decimal TotalCashOnHand { get; set; }
    public int FrozenWallets { get; set; }
    public decimal AvgBalance { get; set; }
    public int TransactionsToday { get; set; }
}

public class WalletTransactionFilterDto : PaginationDto
{
    public TransactionType? TransactionType { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public class WalletExportFilterDto : WalletTransactionFilterDto
{
    public string Format { get; set; } = "csv";
}
