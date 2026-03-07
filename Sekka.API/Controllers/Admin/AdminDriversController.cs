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
[Route("api/v{version:apiVersion}/admin/drivers")]
[Authorize(Roles = "Admin")]
public class AdminDriversController : ControllerBase
{
    private readonly IAdminDriversService _driversService;

    public AdminDriversController(IAdminDriversService driversService)
    {
        _driversService = driversService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDrivers([FromQuery] AdminDriverFilterDto filter)
    {
        var result = await _driversService.GetDriversAsync(filter);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDriver(Guid id)
    {
        var result = await _driversService.GetDriverByIdAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var result = await _driversService.ActivateDriverAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.DriverActivated))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _driversService.DeactivateDriverAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.DriverDeactivated))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("{id:guid}/performance")]
    public async Task<IActionResult> GetPerformance(Guid id, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var result = await _driversService.GetPerformanceAsync(id, fromDate, toDate);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("locations")]
    public async Task<IActionResult> GetLocations()
    {
        var result = await _driversService.GetDriverLocationsAsync();
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
