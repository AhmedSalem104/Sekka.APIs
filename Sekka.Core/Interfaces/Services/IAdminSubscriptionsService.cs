using Sekka.Core.Common;
using Sekka.Core.DTOs.Admin;

namespace Sekka.Core.Interfaces.Services;

public interface IAdminSubscriptionsService
{
    Task<Result<PagedResult<AdminSubscriptionDto>>> GetSubscriptionsAsync(AdminSubscriptionFilterDto filter);
    Task<Result<AdminSubscriptionDetailDto>> GetSubscriptionByIdAsync(Guid id);
    Task<Result<AdminSubscriptionDto>> ExtendSubscriptionAsync(Guid id, ExtendSubscriptionDto dto);
    Task<Result<AdminSubscriptionDto>> CancelSubscriptionAsync(Guid id, CancelSubscriptionDto dto);
    Task<Result<AdminSubscriptionDto>> ChangePlanAsync(Guid id, ChangeSubscriptionPlanDto dto);
    Task<Result<AdminSubscriptionDto>> GiftSubscriptionAsync(GiftSubscriptionDto dto);
    Task<Result<List<SubscriptionPlanDto>>> GetPlansAsync();
    Task<Result<SubscriptionPlanDto>> CreatePlanAsync(CreateSubscriptionPlanDto dto);
    Task<Result<SubscriptionPlanDto>> UpdatePlanAsync(Guid id, UpdateSubscriptionPlanDto dto);
    Task<Result<SubscriptionPlanDto>> TogglePlanAsync(Guid id);
    Task<Result<SubscriptionStatsDto>> GetStatsAsync(DateTime? fromDate, DateTime? toDate);
    Task<Result<List<AdminSubscriptionDto>>> GetExpiringSoonAsync(int? days);
}
