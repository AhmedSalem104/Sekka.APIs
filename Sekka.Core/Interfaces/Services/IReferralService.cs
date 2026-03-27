using Sekka.Core.Common;
using Sekka.Core.DTOs.Social;

namespace Sekka.Core.Interfaces.Services;

public interface IReferralService
{
    Task<Result<ReferralCodeDto>> GetMyCodeAsync(Guid driverId);
    Task<Result<ReferralStatsDto>> GetStatsAsync(Guid driverId);
    Task<Result<ReferralDto>> ApplyCodeAsync(Guid driverId, ApplyReferralCodeDto dto);
    Task<Result<List<ReferralDto>>> GetMyReferralsAsync(Guid driverId);
}
