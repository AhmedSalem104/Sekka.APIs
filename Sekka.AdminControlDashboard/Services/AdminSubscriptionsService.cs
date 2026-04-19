using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.Interfaces.Services;

namespace Sekka.AdminControlDashboard.Services;

public class AdminSubscriptionsService : IAdminSubscriptionsService
{
    private readonly ILogger<AdminSubscriptionsService> _logger;

    public AdminSubscriptionsService(ILogger<AdminSubscriptionsService> logger)
    {
        _logger = logger;
    }

    public Task<Result<PagedResult<AdminSubscriptionDto>>> GetSubscriptionsAsync(AdminSubscriptionFilterDto filter)
    {
        // TODO: Implement when Subscriptions entity is added
        return Task.FromResult(Result<PagedResult<AdminSubscriptionDto>>.Success(
            new PagedResult<AdminSubscriptionDto>(new List<AdminSubscriptionDto>(), 0, filter.Page, filter.PageSize)));
    }

    public Task<Result<AdminSubscriptionDetailDto>> GetSubscriptionByIdAsync(Guid id)
        => Task.FromResult(Result<AdminSubscriptionDetailDto>.NotFound(ErrorMessages.SubscriptionNotFound));

    public Task<Result<AdminSubscriptionDto>> ExtendSubscriptionAsync(Guid id, ExtendSubscriptionDto dto)
        => Task.FromResult(Result<AdminSubscriptionDto>.NotFound(ErrorMessages.SubscriptionNotFound));

    public Task<Result<AdminSubscriptionDto>> CancelSubscriptionAsync(Guid id, CancelSubscriptionDto dto)
        => Task.FromResult(Result<AdminSubscriptionDto>.NotFound(ErrorMessages.SubscriptionNotFound));

    public Task<Result<AdminSubscriptionDto>> ChangePlanAsync(Guid id, ChangeSubscriptionPlanDto dto)
        => Task.FromResult(Result<AdminSubscriptionDto>.NotFound(ErrorMessages.SubscriptionNotFound));

    public Task<Result<AdminSubscriptionDto>> GiftSubscriptionAsync(GiftSubscriptionDto dto)
        => Task.FromResult(Result<AdminSubscriptionDto>.NotImplemented(ErrorMessages.SubscriptionsUnderDev));

    public Task<Result<List<SubscriptionPlanDto>>> GetPlansAsync()
        => Task.FromResult(Result<List<SubscriptionPlanDto>>.Success(new List<SubscriptionPlanDto>()));

    public Task<Result<SubscriptionPlanDto>> CreatePlanAsync(CreateSubscriptionPlanDto dto)
        => Task.FromResult(Result<SubscriptionPlanDto>.NotImplemented(ErrorMessages.PlansUnderDev));

    public Task<Result<SubscriptionPlanDto>> UpdatePlanAsync(Guid id, UpdateSubscriptionPlanDto dto)
        => Task.FromResult(Result<SubscriptionPlanDto>.NotFound(ErrorMessages.PlanNotFound));

    public Task<Result<SubscriptionPlanDto>> TogglePlanAsync(Guid id)
        => Task.FromResult(Result<SubscriptionPlanDto>.NotFound(ErrorMessages.PlanNotFound));

    public Task<Result<SubscriptionStatsDto>> GetStatsAsync(DateTime? fromDate, DateTime? toDate)
        => Task.FromResult(Result<SubscriptionStatsDto>.Success(new SubscriptionStatsDto()));

    public Task<Result<List<AdminSubscriptionDto>>> GetExpiringSoonAsync(int? days)
        => Task.FromResult(Result<List<AdminSubscriptionDto>>.Success(new List<AdminSubscriptionDto>()));
}
