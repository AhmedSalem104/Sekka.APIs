using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.Interfaces.Services;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/subscriptions")]
[Authorize(Roles = "Admin")]
public class AdminSubscriptionsController : ControllerBase
{
    private readonly IAdminSubscriptionsService _subscriptionsService;

    public AdminSubscriptionsController(IAdminSubscriptionsService subscriptionsService)
    {
        _subscriptionsService = subscriptionsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSubscriptions([FromQuery] AdminSubscriptionFilterDto filter)
    {
        var result = await _subscriptionsService.GetSubscriptionsAsync(filter);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSubscription(Guid id)
    {
        var result = await _subscriptionsService.GetSubscriptionByIdAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPut("{id:guid}/extend")]
    public async Task<IActionResult> Extend(Guid id, [FromBody] ExtendSubscriptionDto dto)
    {
        var result = await _subscriptionsService.ExtendSubscriptionAsync(id, dto);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelSubscriptionDto dto)
    {
        var result = await _subscriptionsService.CancelSubscriptionAsync(id, dto);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPut("{id:guid}/change-plan")]
    public async Task<IActionResult> ChangePlan(Guid id, [FromBody] ChangeSubscriptionPlanDto dto)
    {
        var result = await _subscriptionsService.ChangePlanAsync(id, dto);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPost("gift")]
    public async Task<IActionResult> Gift([FromBody] GiftSubscriptionDto dto)
    {
        var result = await _subscriptionsService.GiftSubscriptionAsync(dto);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("plans")]
    public async Task<IActionResult> GetPlans()
    {
        var result = await _subscriptionsService.GetPlansAsync();
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPost("plans")]
    public async Task<IActionResult> CreatePlan([FromBody] CreateSubscriptionPlanDto dto)
    {
        var result = await _subscriptionsService.CreatePlanAsync(dto);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPut("plans/{id:guid}")]
    public async Task<IActionResult> UpdatePlan(Guid id, [FromBody] UpdateSubscriptionPlanDto dto)
    {
        var result = await _subscriptionsService.UpdatePlanAsync(id, dto);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPut("plans/{id:guid}/toggle")]
    public async Task<IActionResult> TogglePlan(Guid id)
    {
        var result = await _subscriptionsService.TogglePlanAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var result = await _subscriptionsService.GetStatsAsync(fromDate, toDate);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("expiring-soon")]
    public async Task<IActionResult> GetExpiringSoon([FromQuery] int? days)
    {
        var result = await _subscriptionsService.GetExpiringSoonAsync(days);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
