using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Customer;
using Sekka.Core.Interfaces.Services;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/blacklist")]
[Authorize(Roles = "Admin")]
public class AdminBlacklistController : ControllerBase
{
    public AdminBlacklistController()
    {
    }

    [HttpGet]
    public IActionResult GetBlacklist([FromQuery] PaginationDto pagination, [FromQuery] string? searchTerm)
    {
        // TODO: Global blacklist across all drivers
        return BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("القائمة السوداء")));
    }

    [HttpPost]
    public IActionResult AddToBlacklist([FromBody] BlockCustomerDto dto)
    {
        // TODO: Add phone/customer to global blacklist
        return BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إضافة للقائمة السوداء")));
    }

    [HttpDelete("{id:guid}")]
    public IActionResult RemoveFromBlacklist(Guid id)
    {
        // TODO: Remove from global blacklist
        return BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إزالة من القائمة السوداء")));
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        // TODO: Blacklist statistics
        return BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إحصائيات القائمة السوداء")));
    }
}
