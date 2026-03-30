using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Auth;
using Sekka.Core.DTOs.Profile;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<Driver> _userManager;
    private readonly ISmsService _smsService;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<Driver> userManager,
        ISmsService smsService,
        IConfiguration config,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _smsService = smsService;
        _config = config;
        _logger = logger;
    }

    public async Task<Result<bool>> SendVerificationAsync(ForgotPasswordDto dto)
    {
        var normalized = EgyptianPhoneHelper.Normalize(dto.PhoneNumber);
        if (!EgyptianPhoneHelper.IsMobile(normalized))
            return Result<bool>.BadRequest(ErrorMessages.InvalidPhoneNumber);

        var existing = await _userManager.FindByNameAsync(normalized);
        if (existing != null)
            return Result<bool>.Conflict(ErrorMessages.PhoneAlreadyRegistered);

        return await _smsService.SendOtpAsync(normalized);
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            return Result<AuthResponseDto>.BadRequest(ErrorMessages.PasswordMismatch);

        var normalized = EgyptianPhoneHelper.Normalize(dto.PhoneNumber);
        if (!EgyptianPhoneHelper.IsMobile(normalized))
            return Result<AuthResponseDto>.BadRequest(ErrorMessages.InvalidPhoneNumber);

        var otpResult = await _smsService.VerifyOtpAsync(normalized, dto.OtpCode);
        if (!otpResult.IsSuccess)
            return Result<AuthResponseDto>.BadRequest(otpResult.Error!.Message);

        var existing = await _userManager.FindByNameAsync(normalized);
        if (existing != null)
            return Result<AuthResponseDto>.Conflict(ErrorMessages.PhoneAlreadyRegistered);

        var driver = new Driver
        {
            UserName = normalized,
            PhoneNumber = normalized,
            PhoneNumberConfirmed = true,
            Name = dto.Name,
            VehicleType = dto.VehicleType,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(driver, dto.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            _logger.LogError("Failed to create driver: {Errors}", errors);
            return Result<AuthResponseDto>.BadRequest(ErrorMessages.AccountCreationFailed(errors));
        }

        await _userManager.AddToRoleAsync(driver, "Driver");

        _logger.LogInformation("Driver {DriverId} registered", driver.Id);
        return Result<AuthResponseDto>.Success(BuildAuthResponse(driver, isNewUser: true));
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var normalized = EgyptianPhoneHelper.Normalize(dto.PhoneNumber);
        var driver = await _userManager.FindByNameAsync(normalized);

        if (driver == null)
            return Result<AuthResponseDto>.Unauthorized(ErrorMessages.InvalidCredentials);

        if (!driver.IsActive)
            return Result<AuthResponseDto>.Unauthorized(ErrorMessages.AccountSuspended);

        var validPassword = await _userManager.CheckPasswordAsync(driver, dto.Password);
        if (!validPassword)
            return Result<AuthResponseDto>.Unauthorized(ErrorMessages.InvalidCredentials);

        _logger.LogInformation("Driver {DriverId} logged in", driver.Id);
        return Result<AuthResponseDto>.Success(BuildAuthResponse(driver, isNewUser: false));
    }

    public async Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var normalized = EgyptianPhoneHelper.Normalize(dto.PhoneNumber);
        var driver = await _userManager.FindByNameAsync(normalized);

        if (driver == null)
            return Result<bool>.Success(true);

        return await _smsService.SendOtpAsync(normalized);
    }

    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return Result<bool>.BadRequest(ErrorMessages.PasswordMismatch);

        var normalized = EgyptianPhoneHelper.Normalize(dto.PhoneNumber);

        var verifyResult = await _smsService.VerifyOtpAsync(normalized, dto.OtpCode);
        if (!verifyResult.IsSuccess)
            return Result<bool>.BadRequest(verifyResult.Error!.Message);

        var driver = await _userManager.FindByNameAsync(normalized);
        if (driver == null)
            return Result<bool>.NotFound(ErrorMessages.AccountNotFound);

        if (await _userManager.HasPasswordAsync(driver))
            await _userManager.RemovePasswordAsync(driver);

        var result = await _userManager.AddPasswordAsync(driver, dto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<bool>.BadRequest(ErrorMessages.SetPasswordFailed(errors));
        }

        _logger.LogInformation("Password reset for driver {DriverId}", driver.Id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ChangePasswordAsync(Guid driverId, ChangePasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return Result<bool>.BadRequest(ErrorMessages.PasswordMismatch);

        var driver = await _userManager.FindByIdAsync(driverId.ToString());
        if (driver == null)
            return Result<bool>.NotFound(ErrorMessages.DriverNotFound);

        var result = await _userManager.ChangePasswordAsync(driver, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<bool>.BadRequest(ErrorMessages.ChangePasswordFailed(errors));
        }

        _logger.LogInformation("Password changed for driver {DriverId}", driverId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var principal = GetPrincipalFromExpiredToken(dto.Token);
        if (principal == null)
            return Result<AuthResponseDto>.Unauthorized(ErrorMessages.InvalidToken);

        var driverId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (driverId == null)
            return Result<AuthResponseDto>.Unauthorized(ErrorMessages.InvalidToken);

        var driver = await _userManager.FindByIdAsync(driverId);
        if (driver == null)
            return Result<AuthResponseDto>.NotFound(ErrorMessages.DriverNotFound);

        return Result<AuthResponseDto>.Success(BuildAuthResponse(driver, isNewUser: false));
    }

    public Task<Result<bool>> LogoutAsync(Guid driverId, string token)
    {
        _logger.LogInformation("Driver {DriverId} logged out", driverId);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public Task<Result<bool>> RegisterDeviceAsync(Guid driverId, RegisterDeviceDto dto)
    {
        _logger.LogInformation("Device registered for driver {DriverId}, platform: {Platform}", driverId, dto.Platform);
        return Task.FromResult(Result<bool>.Success(true));
    }

    // ══════════════════════════════════════════════
    // Private Helpers
    // ══════════════════════════════════════════════

    private AuthResponseDto BuildAuthResponse(Driver driver, bool isNewUser) => new()
    {
        Token = GenerateJwtToken(driver),
        RefreshToken = GenerateRefreshToken(),
        ExpiresAt = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("JwtSettings:AccessTokenExpiryMinutes")),
        IsNewUser = isNewUser,
        Driver = MapToProfileDto(driver)
    };

    private string GenerateJwtToken(Driver driver)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, driver.Id.ToString()),
            new(ClaimTypes.Name, driver.Name),
            new(ClaimTypes.MobilePhone, driver.PhoneNumber ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("AccessTokenExpiryMinutes")),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
        };

        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;
            return principal;
        }
        catch
        {
            return null;
        }
    }

    private static DriverProfileDto MapToProfileDto(Driver driver) => new()
    {
        Id = driver.Id,
        Name = driver.Name,
        Phone = driver.PhoneNumber ?? "",
        Email = driver.Email,
        ProfileImageUrl = driver.ProfileImageUrl,
        LicenseImageUrl = driver.LicenseImageUrl,
        VehicleType = driver.VehicleType ?? Sekka.Core.Enums.VehicleType.Motorcycle,
        IsOnline = driver.IsOnline,
        CashOnHand = driver.CashOnHand,
        TotalPoints = driver.TotalPoints,
        Level = driver.Level,
        JoinedAt = driver.CreatedAt,
        ReferralCode = driver.Id.ToString()[..8].ToUpper()
    };
}
