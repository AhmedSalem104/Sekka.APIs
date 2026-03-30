using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

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

    public async Task<Result<List<ChallengeDto>>> GetActiveChallengesAsync(Guid driverId)
    {
        _logger.LogInformation("GetActiveChallenges for driver {DriverId}", driverId);

        var challengeRepo = _unitOfWork.GetRepository<Challenge, Guid>();
        var achievementRepo = _unitOfWork.GetRepository<DriverAchievement, Guid>();

        var challengeSpec = new ActiveChallengesSpec();
        var challenges = await challengeRepo.ListAsync(challengeSpec);

        var achievementSpec = new DriverAchievementsSpec(driverId);
        var achievements = await achievementRepo.ListAsync(achievementSpec);

        var dtos = challenges.Select(c =>
        {
            var achievement = achievements.FirstOrDefault(a => a.ChallengeId == c.Id);
            return new ChallengeDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ChallengeType = c.ChallengeType,
                TargetValue = c.TargetValue,
                CurrentProgress = achievement?.CurrentProgress ?? 0,
                ProgressPercentage = c.TargetValue > 0
                    ? Math.Min(100, ((achievement?.CurrentProgress ?? 0) / c.TargetValue) * 100)
                    : 0,
                RewardPoints = c.RewardPoints,
                BadgeName = c.BadgeName,
                IsCompleted = achievement?.IsCompleted ?? false
            };
        }).ToList();

        return Result<List<ChallengeDto>>.Success(dtos);
    }

    public async Task<Result<List<DriverAchievementDto>>> GetAchievementsAsync(Guid driverId)
    {
        _logger.LogInformation("GetAchievements for driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<DriverAchievement, Guid>();
        var spec = new CompletedAchievementsSpec(driverId);
        var achievements = await repo.ListAsync(spec);

        var challengeRepo = _unitOfWork.GetRepository<Challenge, Guid>();

        var dtos = new List<DriverAchievementDto>();
        foreach (var a in achievements)
        {
            var challenge = await challengeRepo.GetByIdAsync(a.ChallengeId);
            dtos.Add(new DriverAchievementDto
            {
                Id = a.Id,
                ChallengeName = challenge?.Name ?? "Unknown",
                BadgeName = challenge?.BadgeName,
                BadgeIconUrl = challenge?.BadgeIconUrl,
                PointsEarned = a.PointsEarned,
                CompletedAt = a.CompletedAt ?? a.CreatedAt
            });
        }

        return Result<List<DriverAchievementDto>>.Success(dtos);
    }

    public async Task<Result<LeaderboardDto>> GetLeaderboardAsync(Guid driverId, string period)
    {
        _logger.LogInformation("GetLeaderboard for driver {DriverId}, period {Period}", driverId, period);

        // Return empty leaderboard for now -- requires aggregation across all drivers
        return await Task.FromResult(Result<LeaderboardDto>.Success(new LeaderboardDto
        {
            MyRank = 0,
            MyPoints = 0,
            TopDrivers = new List<LeaderboardEntryDto>()
        }));
    }

    public async Task<Result<bool>> ClaimRewardAsync(Guid driverId, Guid challengeId)
    {
        _logger.LogInformation("ClaimReward for driver {DriverId}, challenge {ChallengeId}", driverId, challengeId);

        var challengeRepo = _unitOfWork.GetRepository<Challenge, Guid>();
        var challenge = await challengeRepo.GetByIdAsync(challengeId);

        if (challenge is null)
            return Result<bool>.NotFound("التحدي غير موجود");

        var achievementRepo = _unitOfWork.GetRepository<DriverAchievement, Guid>();
        var spec = new DriverChallengeAchievementSpec(driverId, challengeId);
        var existing = (await achievementRepo.ListAsync(spec)).FirstOrDefault();

        if (existing is not null && existing.IsCompleted)
            return Result<bool>.Conflict("تم المطالبة بهذه المكافأة بالفعل");

        if (existing is null)
        {
            existing = new DriverAchievement
            {
                Id = Guid.NewGuid(),
                DriverId = driverId,
                ChallengeId = challengeId,
                CurrentProgress = challenge.TargetValue,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow,
                PointsEarned = challenge.RewardPoints
            };
            await achievementRepo.AddAsync(existing);
        }
        else
        {
            existing.IsCompleted = true;
            existing.CompletedAt = DateTime.UtcNow;
            existing.PointsEarned = challenge.RewardPoints;
            achievementRepo.Update(existing);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    public async Task<Result<PagedResult<PointsHistoryDto>>> GetPointsHistoryAsync(Guid driverId, PaginationDto pagination)
    {
        _logger.LogInformation("GetPointsHistory for driver {DriverId}", driverId);

        // Points history is derived from completed achievements
        var repo = _unitOfWork.GetRepository<DriverAchievement, Guid>();
        var spec = new CompletedAchievementsSpec(driverId);
        var achievements = await repo.ListAsync(spec);

        var items = achievements
            .OrderByDescending(a => a.CompletedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(a => new PointsHistoryDto
            {
                Id = a.Id,
                Points = a.PointsEarned,
                Reason = "Challenge completed",
                ReferenceType = "Challenge",
                ReferenceId = a.ChallengeId,
                CreatedAt = a.CompletedAt ?? a.CreatedAt
            }).ToList();

        var paged = new PagedResult<PointsHistoryDto>(items, achievements.Count, pagination.Page, pagination.PageSize);
        return Result<PagedResult<PointsHistoryDto>>.Success(paged);
    }

    public async Task<Result<int>> GetTotalPointsAsync(Guid driverId)
    {
        _logger.LogInformation("GetTotalPoints for driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<DriverAchievement, Guid>();
        var spec = new CompletedAchievementsSpec(driverId);
        var achievements = await repo.ListAsync(spec);

        var total = achievements.Sum(a => a.PointsEarned);
        return Result<int>.Success(total);
    }

    public async Task<Result<int>> GetCurrentLevelAsync(Guid driverId)
    {
        _logger.LogInformation("GetCurrentLevel for driver {DriverId}", driverId);

        var pointsResult = await GetTotalPointsAsync(driverId);
        if (!pointsResult.IsSuccess)
            return Result<int>.Failure(pointsResult.Error!);

        // Simple level calculation: 1 level per 100 points, minimum level 1
        var level = Math.Max(1, (pointsResult.Value / 100) + 1);
        return Result<int>.Success(level);
    }
}

internal class ActiveChallengesSpec : BaseSpecification<Challenge>
{
    public ActiveChallengesSpec(ChallengeType? type = null)
    {
        SetCriteria(c => c.IsActive && (!type.HasValue || c.ChallengeType == type.Value));
        SetOrderBy(c => c.ChallengeType);
    }
}

internal class DriverAchievementsSpec : BaseSpecification<DriverAchievement>
{
    public DriverAchievementsSpec(Guid driverId)
    {
        SetCriteria(a => a.DriverId == driverId);
        SetOrderByDescending(a => a.CreatedAt);
    }
}

internal class CompletedAchievementsSpec : BaseSpecification<DriverAchievement>
{
    public CompletedAchievementsSpec(Guid driverId)
    {
        SetCriteria(a => a.DriverId == driverId && a.IsCompleted);
        SetOrderByDescending(a => a.CompletedAt!);
    }
}

internal class DriverChallengeAchievementSpec : BaseSpecification<DriverAchievement>
{
    public DriverChallengeAchievementSpec(Guid driverId, Guid challengeId)
    {
        SetCriteria(a => a.DriverId == driverId && a.ChallengeId == challengeId);
    }
}
