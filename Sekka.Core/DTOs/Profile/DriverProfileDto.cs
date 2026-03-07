using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Profile;

public class DriverProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? LicenseImageUrl { get; set; }
    public VehicleType VehicleType { get; set; }
    public VehicleInfoDto? ActiveVehicle { get; set; }
    public bool IsOnline { get; set; }
    public string? DefaultRegion { get; set; }
    public decimal CashOnHand { get; set; }
    public decimal WalletBalance { get; set; }
    public int TotalPoints { get; set; }
    public int Level { get; set; }
    public int NextLevelPoints { get; set; }
    public string? SubscriptionPlan { get; set; }
    public DateTime JoinedAt { get; set; }
    public int TotalOrders { get; set; }
    public int TotalDelivered { get; set; }
    public decimal AverageRating { get; set; }
    public ShiftStatus ShiftStatus { get; set; }
    public DateTime? ShiftStartTime { get; set; }
    public int HealthScore { get; set; }
    public int BadgesCount { get; set; }
    public int CurrentStreak { get; set; }
    public int CompletionPercentage { get; set; }
    public int TodayOrdersCount { get; set; }
    public decimal TodayEarnings { get; set; }
    public string ReferralCode { get; set; } = null!;
}
