using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Account;
using Sekka.Core.DTOs.Auth;
using Sekka.Core.Interfaces.Services;
using Enums = Sekka.Core.Enums;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAccountManagementService _accountManagementService;

    public AuthController(IAuthService authService, IAccountManagementService accountManagementService)
    {
        _authService = authService;
        _accountManagementService = accountManagementService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ──────────────────────────────────────────────
    // Registration & Login
    // ──────────────────────────────────────────────

    [HttpGet("vehicle-types")]
    [AllowAnonymous]
    public IActionResult GetVehicleTypes()
    {
        var types = Enum.GetValues<Enums.VehicleType>()
            .Select(v => new { id = (int)v, name = v.ToString() })
            .ToList();
        return Ok(ApiResponse<object>.Success(types));
    }

    [HttpPost("send-verification")]
    [AllowAnonymous]
    [EnableRateLimiting("OtpLimiter")]
    public async Task<IActionResult> SendVerification([FromBody] ForgotPasswordDto dto)
    {
        var result = await _authService.SendVerificationAsync(dto);
        return ToActionResult(result, message: SuccessMessages.OtpSent);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return ToActionResult(result, successCode: StatusCodes.Status201Created, message: SuccessMessages.Registered);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return ToActionResult(result, message: SuccessMessages.LoggedIn);
    }

    // ──────────────────────────────────────────────
    // Password Management
    // ──────────────────────────────────────────────

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [EnableRateLimiting("OtpLimiter")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var result = await _authService.ForgotPasswordAsync(dto);
        return ToActionResult(result, message: SuccessMessages.OtpSent);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [EnableRateLimiting("OtpLimiter")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var result = await _authService.ResetPasswordAsync(dto);
        return ToActionResult(result, message: SuccessMessages.PasswordReset);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var result = await _authService.ChangePasswordAsync(GetDriverId(), dto);
        return ToActionResult(result, message: SuccessMessages.PasswordChanged);
    }

    // ──────────────────────────────────────────────
    // Token Management
    // ──────────────────────────────────────────────

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto);
        return ToActionResult(result, message: SuccessMessages.TokenRefreshed);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var result = await _authService.LogoutAsync(GetDriverId(), token);
        return ToActionResult(result, message: SuccessMessages.LoggedOut);
    }

    // ──────────────────────────────────────────────
    // Device & Sessions
    // ──────────────────────────────────────────────

    [HttpPost("register-device")]
    [Authorize]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceDto dto)
    {
        var result = await _authService.RegisterDeviceAsync(GetDriverId(), dto);
        return ToActionResult(result, message: SuccessMessages.DeviceRegistered);
    }

    [HttpGet("sessions")]
    [Authorize]
    public async Task<IActionResult> GetSessions()
    {
        var result = await _accountManagementService.GetActiveSessionsAsync(GetDriverId());
        return ToActionResult(result);
    }

    [HttpDelete("sessions/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> TerminateSession(Guid id)
    {
        var result = await _accountManagementService.TerminateSessionAsync(GetDriverId(), id);
        return ToActionResult(result, message: SuccessMessages.SessionTerminated);
    }

    [HttpPost("logout-all")]
    [Authorize]
    public async Task<IActionResult> LogoutAll()
    {
        var result = await _accountManagementService.LogoutAllDevicesAsync(GetDriverId());
        return ToActionResult(result, message: SuccessMessages.AllSessionsTerminated);
    }

    // ──────────────────────────────────────────────
    // Account Deletion
    // ──────────────────────────────────────────────

    [HttpDelete("account")]
    [Authorize]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto dto)
    {
        var result = await _accountManagementService.RequestAccountDeletionAsync(GetDriverId(), dto);
        return ToActionResult(result, message: SuccessMessages.DeletionRequested);
    }

    [HttpPost("account/confirm-deletion")]
    [Authorize]
    public async Task<IActionResult> ConfirmDeletion([FromBody] ConfirmDeletionDto dto)
    {
        var result = await _accountManagementService.ConfirmAccountDeletionAsync(GetDriverId(), dto);
        return ToActionResult(result, message: SuccessMessages.DeletionConfirmed);
    }

    // ──────────────────────────────────────────────
    // Helper
    // ──────────────────────────────────────────────

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
