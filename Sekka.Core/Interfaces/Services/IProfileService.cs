using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Profile;

namespace Sekka.Core.Interfaces.Services;

public interface IProfileService
{
    Task<Result<DriverProfileDto>> GetProfileAsync(Guid driverId);
    Task<Result<DriverProfileDto>> UpdateProfileAsync(Guid driverId, UpdateProfileDto dto);
    Task<Result<string>> UploadProfileImageAsync(Guid driverId, Stream imageStream, string fileName);
    Task<Result<bool>> DeleteProfileImageAsync(Guid driverId);
    Task<Result<string>> UploadLicenseImageAsync(Guid driverId, Stream imageStream, string fileName);
    Task<Result<ProfileCompletionDto>> GetCompletionAsync(Guid driverId);
    Task<Result<DriverStatsDto>> GetStatsAsync(Guid driverId, DateTime? fromDate, DateTime? toDate);
    Task<Result<List<BadgeDto>>> GetBadgesAsync(Guid driverId);
    Task<Result<PagedResult<ActivityLogDto>>> GetActivityLogAsync(Guid driverId, PaginationDto pagination);
    Task<Result<List<EmergencyContactDto>>> GetEmergencyContactsAsync(Guid driverId);
    Task<Result<EmergencyContactDto>> AddEmergencyContactAsync(Guid driverId, CreateEmergencyContactDto dto);
    Task<Result<bool>> DeleteEmergencyContactAsync(Guid driverId, Guid contactId);
    Task<Result<SubscriptionDto>> GetSubscriptionAsync(Guid driverId);
    Task<Result<SubscriptionDto>> UpgradeSubscriptionAsync(Guid driverId, UpgradeSubscriptionDto dto);
    Task<Result<List<DriverAchievementDto>>> GetAchievementsAsync(Guid driverId);
    Task<Result<List<ChallengeDto>>> GetChallengesAsync(Guid driverId);
    Task<Result<LeaderboardDto>> GetLeaderboardAsync(Guid driverId);
    Task<Result<PagedResult<ExpenseDto>>> GetExpensesAsync(Guid driverId, ExpenseFilterDto filter);
    Task<Result<ExpenseDto>> AddExpenseAsync(Guid driverId, CreateExpenseDto dto);
}
