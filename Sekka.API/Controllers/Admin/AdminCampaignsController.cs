using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
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
        => Ok(ApiResponse<PagedResult<AdminCampaignDto>>.Success(
            new PagedResult<AdminCampaignDto>(new List<AdminCampaignDto>(), 0, filter.Page, filter.PageSize)));

    [HttpGet("{id:guid}")]
    public IActionResult GetCampaign(Guid id)
        => Ok(ApiResponse<AdminCampaignDetailDto>.Success(new AdminCampaignDetailDto
        {
            Id = id,
            Name = string.Empty,
            CampaignType = string.Empty,
            Channel = string.Empty,
            Status = "Draft",
            MessageTemplate = string.Empty,
            CreatedByName = string.Empty,
            CreatedAt = DateTime.UtcNow
        }));

    [HttpPost]
    public IActionResult CreateCampaign([FromBody] CreateCampaignDto dto)
        => StatusCode(201, ApiResponse<AdminCampaignDto>.Success(new AdminCampaignDto
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            CampaignType = dto.CampaignType,
            Channel = dto.Channel,
            Status = "Draft",
            ScheduledAt = dto.ScheduledAt,
            CreatedAt = DateTime.UtcNow
        }));

    [HttpPut("{id:guid}")]
    public IActionResult UpdateCampaign(Guid id, [FromBody] UpdateCampaignDto dto)
        => Ok(ApiResponse<AdminCampaignDto>.Success(new AdminCampaignDto
        {
            Id = id,
            Name = dto.Name ?? string.Empty,
            CampaignType = string.Empty,
            Channel = dto.Channel ?? string.Empty,
            Status = "Draft",
            ScheduledAt = dto.ScheduledAt,
            CreatedAt = DateTime.UtcNow
        }));

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteCampaign(Guid id)
        => Ok(ApiResponse<bool>.Success(true, "تم حذف الحملة بنجاح"));

    [HttpPost("{id:guid}/launch")]
    public IActionResult LaunchCampaign(Guid id)
        => Ok(ApiResponse<AdminCampaignDto>.Success(new AdminCampaignDto
        {
            Id = id,
            Name = string.Empty,
            CampaignType = string.Empty,
            Channel = string.Empty,
            Status = "Running",
            CreatedAt = DateTime.UtcNow
        }, "تم إطلاق الحملة بنجاح"));

    [HttpPost("{id:guid}/pause")]
    public IActionResult PauseCampaign(Guid id)
        => Ok(ApiResponse<AdminCampaignDto>.Success(new AdminCampaignDto
        {
            Id = id,
            Name = string.Empty,
            CampaignType = string.Empty,
            Channel = string.Empty,
            Status = "Paused",
            CreatedAt = DateTime.UtcNow
        }, "تم إيقاف الحملة بنجاح"));

    [HttpPost("{id:guid}/resume")]
    public IActionResult ResumeCampaign(Guid id)
        => Ok(ApiResponse<AdminCampaignDto>.Success(new AdminCampaignDto
        {
            Id = id,
            Name = string.Empty,
            CampaignType = string.Empty,
            Channel = string.Empty,
            Status = "Running",
            CreatedAt = DateTime.UtcNow
        }, "تم استئناف الحملة بنجاح"));

    [HttpGet("stats")]
    public IActionResult GetStats()
        => Ok(ApiResponse<CampaignStatsDto>.Success(new CampaignStatsDto()));

    [HttpGet("{id:guid}/analytics")]
    public IActionResult GetCampaignAnalytics(Guid id)
        => Ok(ApiResponse<CampaignAnalyticsDto>.Success(new CampaignAnalyticsDto()));
}
