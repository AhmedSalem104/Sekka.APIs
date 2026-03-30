using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Social;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/savings-circles")]
[Authorize(Roles = "Admin")]
public class AdminSavingsCirclesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll([FromQuery] PaginationDto pagination)
        => Ok(ApiResponse<PagedResult<CircleDto>>.Success(
            new PagedResult<CircleDto>(new List<CircleDto>(), 0, pagination.Page, pagination.PageSize)));

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
        => Ok(ApiResponse<CircleDetailDto>.Success(new CircleDetailDto
        {
            Id = id,
            Name = string.Empty
        }));

    [HttpPost("{id:guid}/approve")]
    public IActionResult Approve(Guid id)
        => Ok(ApiResponse<bool>.Success(true, "تم اعتماد حلقة التوفير بنجاح"));

    [HttpPost("{id:guid}/reject")]
    public IActionResult Reject(Guid id)
        => Ok(ApiResponse<bool>.Success(true, "تم رفض حلقة التوفير"));

    [HttpPost("{id:guid}/freeze")]
    public IActionResult Freeze(Guid id)
        => Ok(ApiResponse<bool>.Success(true, "تم تجميد حلقة التوفير"));

    [HttpPost("{id:guid}/unfreeze")]
    public IActionResult Unfreeze(Guid id)
        => Ok(ApiResponse<bool>.Success(true, "تم إلغاء تجميد حلقة التوفير"));

    [HttpPost("{id:guid}/close")]
    public IActionResult Close(Guid id)
        => Ok(ApiResponse<bool>.Success(true, "تم إغلاق حلقة التوفير"));

    [HttpGet("{id:guid}/members")]
    public IActionResult GetMembers(Guid id)
        => Ok(ApiResponse<List<CircleMemberDto>>.Success(new List<CircleMemberDto>()));

    [HttpDelete("{id:guid}/members/{memberId:guid}")]
    public IActionResult RemoveMember(Guid id, Guid memberId)
        => Ok(ApiResponse<bool>.Success(true, "تم إزالة العضو من حلقة التوفير"));

    [HttpGet("{id:guid}/payments")]
    public IActionResult GetPayments(Guid id)
        => Ok(ApiResponse<List<CirclePaymentDto>>.Success(new List<CirclePaymentDto>()));

    [HttpGet("stats")]
    public IActionResult GetStats()
        => Ok(ApiResponse<object>.Success(new
        {
            TotalCircles = 0,
            ActiveCircles = 0,
            FrozenCircles = 0,
            CompletedCircles = 0,
            TotalMembers = 0,
            TotalMonthlyAmount = 0m
        }));
}
