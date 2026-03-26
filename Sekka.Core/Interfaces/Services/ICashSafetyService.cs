using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;

namespace Sekka.Core.Interfaces.Services;

public interface ICashSafetyService
{
    Task<Result<CashStatusDto>> GetCashStatusAsync(Guid driverId);
}
