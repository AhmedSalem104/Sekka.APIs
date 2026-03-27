using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
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
    public Task<IActionResult> GetAll([FromQuery] PaginationDto pagination)
        => Stub<PagedResult<CircleDto>>();

    [HttpGet("{id:guid}")]
    public Task<IActionResult> GetById(Guid id)
        => Stub<CircleDetailDto>();

    [HttpPost("{id:guid}/approve")]
    public Task<IActionResult> Approve(Guid id)
        => Stub<bool>();

    [HttpPost("{id:guid}/reject")]
    public Task<IActionResult> Reject(Guid id)
        => Stub<bool>();

    [HttpPost("{id:guid}/freeze")]
    public Task<IActionResult> Freeze(Guid id)
        => Stub<bool>();

    [HttpPost("{id:guid}/unfreeze")]
    public Task<IActionResult> Unfreeze(Guid id)
        => Stub<bool>();

    [HttpPost("{id:guid}/close")]
    public Task<IActionResult> Close(Guid id)
        => Stub<bool>();

    [HttpGet("{id:guid}/members")]
    public Task<IActionResult> GetMembers(Guid id)
        => Stub<List<CircleMemberDto>>();

    [HttpDelete("{id:guid}/members/{memberId:guid}")]
    public Task<IActionResult> RemoveMember(Guid id, Guid memberId)
        => Stub<bool>();

    [HttpGet("{id:guid}/payments")]
    public Task<IActionResult> GetPayments(Guid id)
        => Stub<List<CirclePaymentDto>>();

    [HttpGet("stats")]
    public Task<IActionResult> GetStats()
        => Stub<object>();

    private Task<IActionResult> Stub<T>()
    {
        var result = Result<T>.BadRequest(ErrorMessages.FeatureUnderDevelopment("إدارة حلقات التوفير"));
        return Task.FromResult(ToActionResult(result));
    }

    private IActionResult ToActionResult<T>(Result<T> result, int successCode = 200, string? message = null)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!, message));

        return result.Error!.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.Fail(result.Error.Message)),
            "UNAUTHORIZED" => Unauthorized(ApiResponse<T>.Fail(result.Error.Message)),
            "CONFLICT" => Conflict(ApiResponse<T>.Fail(result.Error.Message)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Error.Message))
        };
    }
}
