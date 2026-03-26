using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;

namespace Sekka.Core.Interfaces.Services;

public interface ISettlementService
{
    Task<Result<List<SettlementDto>>> GetSettlementsAsync(Guid driverId, SettlementFilterDto filter);
    Task<Result<SettlementDto>> CreateAsync(Guid driverId, CreateSettlementDto dto);
    Task<Result<PartnerBalanceDto>> GetPartnerBalanceAsync(Guid driverId, Guid partnerId);
    Task<Result<DailySettlementSummaryDto>> GetDailySummaryAsync(Guid driverId);
    Task<Result<bool>> UploadReceiptAsync(Guid driverId, Guid id, Stream stream, string fileName);
}
