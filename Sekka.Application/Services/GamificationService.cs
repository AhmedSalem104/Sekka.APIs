using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class GamificationService : IGamificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GamificationService> _logger;

    public GamificationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GamificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<Result<List<ChallengeDto>>> GetActiveChallengesAsync(Guid driverId)
    {
        _logger.LogWarning("GetActiveChallenges called — feature under development");
        return Task.FromResult(Result<List<ChallengeDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("التحديات")));
    }

    public Task<Result<List<DriverAchievementDto>>> GetAchievementsAsync(Guid driverId)
    {
        _logger.LogWarning("GetAchievements called — feature under development");
        return Task.FromResult(Result<List<DriverAchievementDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الإنجازات")));
    }

    public Task<Result<LeaderboardDto>> GetLeaderboardAsync(Guid driverId, string period)
    {
        _logger.LogWarning("GetLeaderboard called — feature under development");
        return Task.FromResult(Result<LeaderboardDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("لوحة المتصدرين")));
    }

    public Task<Result<bool>> ClaimRewardAsync(Guid driverId, Guid challengeId)
    {
        _logger.LogWarning("ClaimReward called — feature under development");
        return Task.FromResult(Result<bool>.BadRequest(ErrorMessages.FeatureUnderDevelopment("المكافآت")));
    }

    public Task<Result<PagedResult<PointsHistoryDto>>> GetPointsHistoryAsync(Guid driverId, PaginationDto pagination)
    {
        _logger.LogWarning("GetPointsHistory called — feature under development");
        return Task.FromResult(Result<PagedResult<PointsHistoryDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("سجل النقاط")));
    }

    public Task<Result<int>> GetTotalPointsAsync(Guid driverId)
    {
        _logger.LogWarning("GetTotalPoints called — feature under development");
        return Task.FromResult(Result<int>.BadRequest(ErrorMessages.FeatureUnderDevelopment("النقاط")));
    }

    public Task<Result<int>> GetCurrentLevelAsync(Guid driverId)
    {
        _logger.LogWarning("GetCurrentLevel called — feature under development");
        return Task.FromResult(Result<int>.BadRequest(ErrorMessages.FeatureUnderDevelopment("المستوى")));
    }
}
