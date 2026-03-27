using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Social;

namespace Sekka.Core.Interfaces.Services;

public interface IGamificationService
{
    Task<Result<List<ChallengeDto>>> GetActiveChallengesAsync(Guid driverId);
    Task<Result<List<DriverAchievementDto>>> GetAchievementsAsync(Guid driverId);
    Task<Result<LeaderboardDto>> GetLeaderboardAsync(Guid driverId, string period);
    Task<Result<bool>> ClaimRewardAsync(Guid driverId, Guid challengeId);
    Task<Result<PagedResult<PointsHistoryDto>>> GetPointsHistoryAsync(Guid driverId, PaginationDto pagination);
    Task<Result<int>> GetTotalPointsAsync(Guid driverId);
    Task<Result<int>> GetCurrentLevelAsync(Guid driverId);
}
