using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Badge;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class BadgeService : IBadgeService
{
    private readonly UserManager<Driver> _userManager;
    private readonly ILogger<BadgeService> _logger;

    public BadgeService(UserManager<Driver> userManager, ILogger<BadgeService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<DigitalBadgeDto>> GetDigitalBadgeAsync(Guid driverId)
    {
        var driver = await _userManager.FindByIdAsync(driverId.ToString());
        if (driver == null)
            return Result<DigitalBadgeDto>.NotFound(ErrorMessages.DriverNotFound);

        return Result<DigitalBadgeDto>.Success(new DigitalBadgeDto
        {
            DriverName = driver.Name,
            ProfileImageUrl = driver.ProfileImageUrl,
            DriverId = driver.Id,
            VehicleType = driver.VehicleType ?? Sekka.Core.Enums.VehicleType.Motorcycle,
            AverageRating = 0,
            TotalDeliveries = 0,
            MemberSince = driver.CreatedAt,
            Level = driver.Level,
            QrCodeToken = Convert.ToBase64String(driver.Id.ToByteArray()),
            IsVerified = driver.PhoneNumberConfirmed
        });
    }

    public async Task<Result<BadgeVerificationDto>> VerifyBadgeAsync(string qrToken)
    {
        try
        {
            var driverIdBytes = Convert.FromBase64String(qrToken);
            var driverId = new Guid(driverIdBytes);
            var driver = await _userManager.FindByIdAsync(driverId.ToString());

            if (driver == null)
                return Result<BadgeVerificationDto>.Success(new BadgeVerificationDto { IsValid = false, VerifiedAt = DateTime.UtcNow });

            return Result<BadgeVerificationDto>.Success(new BadgeVerificationDto
            {
                IsValid = true,
                DriverName = driver.Name,
                VehicleType = driver.VehicleType ?? Sekka.Core.Enums.VehicleType.Motorcycle,
                Rating = 0,
                IsActive = driver.IsActive,
                VerifiedAt = DateTime.UtcNow
            });
        }
        catch
        {
            return Result<BadgeVerificationDto>.Success(new BadgeVerificationDto { IsValid = false, VerifiedAt = DateTime.UtcNow });
        }
    }
}
