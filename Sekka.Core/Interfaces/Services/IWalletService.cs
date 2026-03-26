using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;

namespace Sekka.Core.Interfaces.Services;

public interface IWalletService
{
    Task<Result<WalletBalanceDto>> GetBalanceAsync(Guid driverId);
    Task<Result<List<WalletTransactionDto>>> GetTransactionsAsync(Guid driverId, WalletTransactionFilterDto filter);
    Task<Result<WalletSummaryDto>> GetSummaryAsync(Guid driverId);
    Task<Result<CashStatusDto>> GetCashStatusAsync(Guid driverId);
}
