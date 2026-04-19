using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Vehicle;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/vehicles")]
[Authorize]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetVehicles()
        => ToActionResult(await _vehicleService.GetVehiclesAsync(GetDriverId()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVehicleDto dto)
        => ToActionResult(await _vehicleService.CreateAsync(GetDriverId(), dto), StatusCodes.Status201Created);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleDto dto)
        => ToActionResult(await _vehicleService.UpdateAsync(GetDriverId(), id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => ToActionResult(await _vehicleService.DeleteAsync(GetDriverId(), id));

    [HttpPost("{vehicleId:guid}/maintenance")]
    public async Task<IActionResult> AddMaintenance(Guid vehicleId, [FromBody] CreateMaintenanceDto dto)
        => ToActionResult(await _vehicleService.AddMaintenanceAsync(GetDriverId(), vehicleId, dto), StatusCodes.Status201Created);

    [HttpGet("{vehicleId:guid}/maintenance")]
    public async Task<IActionResult> GetMaintenance(Guid vehicleId)
        => ToActionResult(await _vehicleService.GetMaintenanceAsync(GetDriverId(), vehicleId));

    private IActionResult ToActionResult<T>(Result<T> result, int successCode = 200, string? message = null)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!, message));

        return result.Error!.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.Fail(result.Error.Message)),
            "UNAUTHORIZED" => Unauthorized(ApiResponse<T>.Fail(result.Error.Message)),
            "CONFLICT" => Conflict(ApiResponse<T>.Fail(result.Error.Message)),
            "NOT_IMPLEMENTED" => StatusCode(501, ApiResponse<T>.Fail(result.Error.Message)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Error.Message))
        };
    }
}
