using Sekka.Core.Common;
using Sekka.Core.DTOs.HealthScore;

namespace Sekka.Core.Interfaces.Services;

public interface IHealthScoreService
{
    Task<Result<AccountHealthDto>> GetHealthScoreAsync(Guid driverId);
    Task<Result<List<HealthTipDto>>> GetHealthTipsAsync(Guid driverId);
}
