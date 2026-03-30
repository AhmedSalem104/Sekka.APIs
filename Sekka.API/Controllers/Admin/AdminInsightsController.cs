using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Admin;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/insights")]
[Authorize(Roles = "Admin")]
public class AdminInsightsController : ControllerBase
{
    [HttpGet("overview")]
    public IActionResult GetOverview()
        => Ok(ApiResponse<InsightsOverviewDto>.Success(new InsightsOverviewDto()));

    [HttpGet("heatmap")]
    public IActionResult GetHeatmap()
        => Ok(ApiResponse<InterestHeatmapDto>.Success(new InterestHeatmapDto()));

    [HttpGet("trends")]
    public IActionResult GetTrends([FromQuery] TrendsQueryDto query)
        => Ok(ApiResponse<List<InterestTrendDto>>.Success(new List<InterestTrendDto>()));

    [HttpGet("engagement-distribution")]
    public IActionResult GetEngagementDistribution()
        => Ok(ApiResponse<EngagementDistributionDto>.Success(new EngagementDistributionDto()));

    [HttpGet("rfm-analysis")]
    public IActionResult GetRfmAnalysis()
        => Ok(ApiResponse<RfmAnalysisDto>.Success(new RfmAnalysisDto()));

    [HttpGet("behavior-summary")]
    public IActionResult GetGlobalBehaviorSummary()
        => Ok(ApiResponse<GlobalBehaviorSummaryDto>.Success(new GlobalBehaviorSummaryDto
        {
            MostPopularOrderTime = string.Empty,
            MostPopularDayOfWeek = string.Empty,
            MostPopularPaymentMethod = string.Empty
        }));

    [HttpGet("category-performance")]
    public IActionResult GetCategoryPerformance()
        => Ok(ApiResponse<List<CategoryPerformanceDto>>.Success(new List<CategoryPerformanceDto>()));
}
