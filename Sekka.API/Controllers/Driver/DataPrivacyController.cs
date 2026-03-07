using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Privacy;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/privacy")]
[Authorize]
public class DataPrivacyController : ControllerBase
{
    private readonly IDataPrivacyService _privacyService;

    public DataPrivacyController(IDataPrivacyService privacyService)
    {
        _privacyService = privacyService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("consents")]
    public async Task<IActionResult> GetConsents()
    {
        var result = await _privacyService.GetConsentsAsync(GetDriverId());
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPut("consents/{type}")]
    public async Task<IActionResult> UpdateConsent(string type, [FromBody] UpdateConsentDto dto)
    {
        var result = await _privacyService.UpdateConsentAsync(GetDriverId(), type, dto);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.ConsentUpdated))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPost("export-data")]
    public async Task<IActionResult> ExportData()
    {
        var result = await _privacyService.RequestDataExportAsync(GetDriverId());
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.DataExportRequested))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPost("delete-data")]
    public async Task<IActionResult> DeleteData([FromBody] DeletionRequestDto dto)
    {
        var result = await _privacyService.RequestDataDeletionAsync(GetDriverId(), dto);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.DataDeletionRequested))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("delete-data/status")]
    public async Task<IActionResult> GetDeletionStatus()
    {
        var result = await _privacyService.GetDeletionStatusAsync(GetDriverId());
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
