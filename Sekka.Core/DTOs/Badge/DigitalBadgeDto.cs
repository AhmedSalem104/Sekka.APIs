using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Badge;

public class DigitalBadgeDto
{
    public string DriverName { get; set; } = null!;
    public string? ProfileImageUrl { get; set; }
    public Guid DriverId { get; set; }
    public VehicleType VehicleType { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalDeliveries { get; set; }
    public DateTime MemberSince { get; set; }
    public int Level { get; set; }
    public string QrCodeToken { get; set; } = null!;
    public bool IsVerified { get; set; }
}

public class BadgeVerificationDto
{
    public bool IsValid { get; set; }
    public string? DriverName { get; set; }
    public VehicleType? VehicleType { get; set; }
    public decimal? Rating { get; set; }
    public bool IsActive { get; set; }
    public DateTime VerifiedAt { get; set; }
}
