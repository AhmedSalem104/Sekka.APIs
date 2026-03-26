using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.Interfaces.Services;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/vehicles")]
[Authorize(Roles = "Admin")]
public class AdminVehiclesController : ControllerBase
{
    private readonly IAdminVehicleService _adminVehicleService;

    public AdminVehiclesController(IAdminVehicleService adminVehicleService)
    {
        _adminVehicleService = adminVehicleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetVehicles([FromQuery] AdminVehicleFilterDto filter)
        => ToActionResult(await _adminVehicleService.GetVehiclesAsync(filter));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => ToActionResult(await _adminVehicleService.GetByIdAsync(id));

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id)
        => ToActionResult(await _adminVehicleService.ApproveAsync(id));

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectVehicleDto dto)
        => ToActionResult(await _adminVehicleService.RejectAsync(id, dto));

    [HttpPost("{id:guid}/flag-maintenance")]
    public async Task<IActionResult> FlagMaintenance(Guid id, [FromBody] FlagMaintenanceDto dto)
        => ToActionResult(await _adminVehicleService.FlagMaintenanceAsync(id, dto));

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
        => ToActionResult(await _adminVehicleService.DeactivateAsync(id));

    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
        => ToActionResult(await _adminVehicleService.ActivateAsync(id));

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
        => ToActionResult(await _adminVehicleService.GetPendingAsync());

    [HttpGet("maintenance-due")]
    public async Task<IActionResult> GetMaintenanceDue()
        => ToActionResult(await _adminVehicleService.GetMaintenanceDueAsync());

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] DateOnly? dateFrom, [FromQuery] DateOnly? dateTo)
        => ToActionResult(await _adminVehicleService.GetStatsAsync(dateFrom, dateTo));

    [HttpGet("by-type")]
    public async Task<IActionResult> GetByType()
        => ToActionResult(await _adminVehicleService.GetByTypeAsync());

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
