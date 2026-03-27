using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
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
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("نظرة عامة على الذكاء")));

    [HttpGet("heatmap")]
    public IActionResult GetHeatmap()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("خريطة حرارية للاهتمامات")));

    [HttpGet("trends")]
    public IActionResult GetTrends([FromQuery] TrendsQueryDto query)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("اتجاهات الاهتمامات")));

    [HttpGet("engagement-distribution")]
    public IActionResult GetEngagementDistribution()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("توزيع التفاعل")));

    [HttpGet("rfm-analysis")]
    public IActionResult GetRfmAnalysis()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تحليل RFM")));

    [HttpGet("behavior-summary")]
    public IActionResult GetGlobalBehaviorSummary()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("ملخص السلوك العام")));

    [HttpGet("category-performance")]
    public IActionResult GetCategoryPerformance()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("أداء الفئات")));
}
