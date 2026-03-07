using Sekka.Core.Common;
using Sekka.Core.DTOs.Badge;

namespace Sekka.Core.Interfaces.Services;

public interface IBadgeService
{
    Task<Result<DigitalBadgeDto>> GetDigitalBadgeAsync(Guid driverId);
    Task<Result<BadgeVerificationDto>> VerifyBadgeAsync(string qrToken);
}
