using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.DTOs.Common;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/campaigns")]
[Authorize(Roles = "Admin")]
public class AdminCampaignsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetCampaigns([FromQuery] CampaignFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("قائمة الحملات")));

    [HttpGet("{id:guid}")]
    public IActionResult GetCampaign(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تفاصيل الحملة")));

    [HttpPost]
    public IActionResult CreateCampaign([FromBody] CreateCampaignDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إنشاء حملة")));

    [HttpPut("{id:guid}")]
    public IActionResult UpdateCampaign(Guid id, [FromBody] UpdateCampaignDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تعديل حملة")));

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteCampaign(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("حذف حملة")));

    [HttpPost("{id:guid}/launch")]
    public IActionResult LaunchCampaign(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إطلاق حملة")));

    [HttpPost("{id:guid}/pause")]
    public IActionResult PauseCampaign(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إيقاف حملة")));

    [HttpPost("{id:guid}/resume")]
    public IActionResult ResumeCampaign(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("استئناف حملة")));

    [HttpGet("stats")]
    public IActionResult GetStats()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إحصائيات الحملات")));

    [HttpGet("{id:guid}/analytics")]
    public IActionResult GetCampaignAnalytics(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تحليلات الحملة")));
}
