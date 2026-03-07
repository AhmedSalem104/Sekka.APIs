using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.Interfaces.Services;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/roles")]
[Authorize(Roles = "Admin")]
public class AdminRolesController : ControllerBase
{
    private readonly IAdminRolesService _rolesService;

    public AdminRolesController(IAdminRolesService rolesService)
    {
        _rolesService = rolesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var result = await _rolesService.GetRolesAsync();
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
    {
        var result = await _rolesService.CreateRoleAsync(dto);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<object>.Success(result.Value!, SuccessMessages.RoleCreated))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleDto dto)
    {
        var result = await _rolesService.UpdateRoleAsync(id, dto);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.RoleUpdated))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var result = await _rolesService.DeleteRoleAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.RoleDeleted))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        var result = await _rolesService.AssignRoleAsync(dto);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.RoleAssigned))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpDelete("revoke")]
    public async Task<IActionResult> RevokeRole([FromBody] RevokeRoleDto dto)
    {
        var result = await _rolesService.RevokeRoleAsync(dto);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.RoleRemoved))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("users/{userId:guid}")]
    public async Task<IActionResult> GetUserRoles(Guid userId)
    {
        var result = await _rolesService.GetUserRolesAsync(userId);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
