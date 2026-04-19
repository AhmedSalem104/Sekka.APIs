using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Intelligence;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customer-insights")]
[Authorize]
public class CustomerInsightsController : ControllerBase
{
    private readonly IInterestEngineService _interestEngineService;
    private readonly IRecommendationService _recommendationService;
    private readonly IBehaviorAnalysisService _behaviorAnalysisService;

    public CustomerInsightsController(
        IInterestEngineService interestEngineService,
        IRecommendationService recommendationService,
        IBehaviorAnalysisService behaviorAnalysisService)
    {
        _interestEngineService = interestEngineService;
        _recommendationService = recommendationService;
        _behaviorAnalysisService = behaviorAnalysisService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Get full interest profile for a customer
    /// </summary>
    [HttpGet("{customerId:guid}/profile")]
    public async Task<IActionResult> GetProfile(Guid customerId)
        => ToActionResult(await _interestEngineService.GetCustomerProfileAsync(GetDriverId(), customerId));

    /// <summary>
    /// Get customer interests (category scores)
    /// </summary>
    [HttpGet("{customerId:guid}/interests")]
    public async Task<IActionResult> GetInterests(Guid customerId)
        => ToActionResult(await _interestEngineService.GetCustomerInterestsAsync(GetDriverId(), customerId));

    /// <summary>
    /// Get recommendations for a customer
    /// </summary>
    [HttpGet("{customerId:guid}/recommendations")]
    public async Task<IActionResult> GetRecommendations(Guid customerId)
        => ToActionResult(await _recommendationService.GetRecommendationsAsync(GetDriverId(), customerId));

    /// <summary>
    /// Mark a recommendation as read
    /// </summary>
    [HttpPut("recommendations/{recommendationId:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid recommendationId)
        => ToActionResult(await _recommendationService.MarkReadAsync(GetDriverId(), recommendationId));

    /// <summary>
    /// Dismiss a recommendation
    /// </summary>
    [HttpPut("recommendations/{recommendationId:guid}/dismiss")]
    public async Task<IActionResult> Dismiss(Guid recommendationId)
        => ToActionResult(await _recommendationService.DismissAsync(GetDriverId(), recommendationId));

    /// <summary>
    /// Mark a recommendation as acted upon
    /// </summary>
    [HttpPut("recommendations/{recommendationId:guid}/act")]
    public async Task<IActionResult> MarkActedUpon(Guid recommendationId)
        => ToActionResult(await _recommendationService.MarkActedUponAsync(GetDriverId(), recommendationId));

    /// <summary>
    /// Get behavior summary for a customer
    /// </summary>
    [HttpGet("{customerId:guid}/behavior")]
    public async Task<IActionResult> GetBehavior(Guid customerId)
        => ToActionResult(await _behaviorAnalysisService.GetBehaviorSummaryAsync(GetDriverId(), customerId));

    /// <summary>
    /// Get top interest categories across all customers
    /// </summary>
    [HttpGet("top-interests")]
    public async Task<IActionResult> GetTopInterests([FromQuery] TopInterestsQueryDto query)
        => ToActionResult(await _interestEngineService.GetTopInterestsAsync(GetDriverId(), query));

    /// <summary>
    /// Get customer segments summary
    /// </summary>
    [HttpGet("segments")]
    public async Task<IActionResult> GetSegments()
        => ToActionResult(await _interestEngineService.GetSegmentsAsync(GetDriverId()));

    /// <summary>
    /// Get customers in a specific segment
    /// </summary>
    [HttpGet("segments/{segmentId:guid}/customers")]
    public async Task<IActionResult> GetSegmentCustomers(Guid segmentId, [FromQuery] PaginationDto pagination)
        => ToActionResult(await _interestEngineService.GetSegmentCustomersAsync(GetDriverId(), segmentId, pagination));

    private IActionResult ToActionResult<T>(Result<T> result, int successCode = 200, string? message = null)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!, message));

        return result.Error!.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.Fail(result.Error.Message)),
            "UNAUTHORIZED" => Unauthorized(ApiResponse<T>.Fail(result.Error.Message)),
            "CONFLICT" => Conflict(ApiResponse<T>.Fail(result.Error.Message)),
            "NOT_IMPLEMENTED" => StatusCode(501, ApiResponse<T>.Fail(result.Error.Message)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Error.Message))
        };
    }
}
