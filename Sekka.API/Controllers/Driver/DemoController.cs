using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Auth;
using Sekka.Core.Interfaces.Services;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/demo")]
public class DemoController : ControllerBase
{
    private readonly IDemoService _demoService;

    public DemoController(IDemoService demoService)
    {
        _demoService = demoService;
    }

    [HttpPost("start")]
    [AllowAnonymous]
    public async Task<IActionResult> Start()
    {
        var result = await _demoService.StartDemoAsync();
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("data")]
    [Authorize]
    public async Task<IActionResult> GetData([FromQuery] Guid sessionId)
    {
        var result = await _demoService.GetDemoDataAsync(sessionId);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPost("end")]
    [Authorize]
    public async Task<IActionResult> End([FromQuery] Guid sessionId)
    {
        var result = await _demoService.EndDemoAsync(sessionId);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPost("convert")]
    [Authorize]
    public async Task<IActionResult> Convert([FromQuery] Guid sessionId, [FromBody] CompleteRegistrationDto dto)
    {
        var result = await _demoService.ConvertToRealAccountAsync(sessionId, dto);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
