using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Customer;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/addresses")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] AddressSearchDto search)
        => ToActionResult(await _addressService.SearchAsync(GetDriverId(), search));

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] SaveAddressDto dto)
        => ToActionResult(await _addressService.SaveAsync(GetDriverId(), dto), StatusCodes.Status201Created);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAddressDto dto)
        => ToActionResult(await _addressService.UpdateAsync(GetDriverId(), id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => ToActionResult(await _addressService.DeleteAsync(GetDriverId(), id));

    [HttpGet("autocomplete")]
    public async Task<IActionResult> Autocomplete([FromQuery] string q, [FromQuery] double? latitude, [FromQuery] double? longitude)
        => ToActionResult(await _addressService.AutocompleteAsync(GetDriverId(), q, latitude, longitude));

    [HttpGet("nearby")]
    public async Task<IActionResult> Nearby([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radiusKm = 5)
        => ToActionResult(await _addressService.NearbyAsync(GetDriverId(), latitude, longitude, radiusKm));

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
