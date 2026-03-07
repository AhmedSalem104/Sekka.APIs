using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Enums;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lookups")]
[AllowAnonymous]
public class LookupsController : ControllerBase
{
    [HttpGet("vehicle-types")]
    public IActionResult GetVehicleTypes()
    {
        var types = Enum.GetValues<VehicleType>()
            .Select(v => new { id = (int)v, name = v.ToString() })
            .ToList();

        return Ok(ApiResponse<object>.Success(types));
    }
}
