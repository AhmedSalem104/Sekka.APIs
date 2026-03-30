using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.DTOs.Common;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/segments")]
[Authorize(Roles = "Admin")]
public class AdminSegmentsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetSegments([FromQuery] SegmentFilterDto filter)
        => Ok(ApiResponse<PagedResult<AdminSegmentDto>>.Success(
            new PagedResult<AdminSegmentDto>(new List<AdminSegmentDto>(), 0, filter.Page, filter.PageSize)));

    [HttpGet("{id:guid}")]
    public IActionResult GetSegment(Guid id)
        => Ok(ApiResponse<AdminSegmentDetailDto>.Success(new AdminSegmentDetailDto
        {
            Id = id,
            Name = string.Empty,
            NameAr = string.Empty,
            SegmentType = string.Empty
        }));

    [HttpPost]
    public IActionResult CreateSegment([FromBody] CreateSegmentDto dto)
        => StatusCode(201, ApiResponse<AdminSegmentDto>.Success(new AdminSegmentDto
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            NameAr = dto.NameAr,
            SegmentType = dto.SegmentType,
            Description = dto.Description,
            ColorHex = dto.ColorHex,
            IsAutomatic = dto.IsAutomatic,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        }));

    [HttpPut("{id:guid}")]
    public IActionResult UpdateSegment(Guid id, [FromBody] UpdateSegmentDto dto)
        => Ok(ApiResponse<AdminSegmentDto>.Success(new AdminSegmentDto
        {
            Id = id,
            Name = dto.Name ?? string.Empty,
            NameAr = dto.NameAr ?? string.Empty,
            SegmentType = dto.SegmentType ?? string.Empty,
            Description = dto.Description,
            ColorHex = dto.ColorHex,
            IsAutomatic = dto.IsAutomatic ?? false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        }));

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteSegment(Guid id)
        => Ok(ApiResponse<bool>.Success(true, "تم حذف الشريحة بنجاح"));

    [HttpPost("{id:guid}/refresh")]
    public IActionResult RefreshSegment(Guid id)
        => Ok(ApiResponse<bool>.Success(true, "تم تحديث الشريحة بنجاح"));

    [HttpGet("{id:guid}/members")]
    public IActionResult GetMembers(Guid id, [FromQuery] PaginationDto pagination)
        => Ok(ApiResponse<PagedResult<SegmentMemberDto>>.Success(
            new PagedResult<SegmentMemberDto>(new List<SegmentMemberDto>(), 0, pagination.Page, pagination.PageSize)));

    [HttpPost("{id:guid}/members/{customerId:guid}")]
    public IActionResult AddMember(Guid id, Guid customerId)
        => Ok(ApiResponse<bool>.Success(true, "تم إضافة العضو بنجاح"));

    [HttpDelete("{id:guid}/members/{customerId:guid}")]
    public IActionResult RemoveMember(Guid id, Guid customerId)
        => Ok(ApiResponse<bool>.Success(true, "تم إزالة العضو بنجاح"));

    [HttpGet("analytics")]
    public IActionResult GetAnalytics()
        => Ok(ApiResponse<SegmentAnalyticsDto>.Success(new SegmentAnalyticsDto()));
}
