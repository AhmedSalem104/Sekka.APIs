using Microsoft.AspNetCore.Identity;
using Sekka.Core.Enums;

namespace Sekka.Persistence.Entities;

public class Driver : IdentityUser<Guid>
{
    public string Name { get; set; } = null!;
    public string? ProfileImageUrl { get; set; }
    public VehicleType VehicleType { get; set; }
    public Guid? DefaultRegionId { get; set; }
    public Guid? SubscriptionPlanId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsOnline { get; set; }
    public double? LastKnownLatitude { get; set; }
    public double? LastKnownLongitude { get; set; }
    public DateTime? LastLocationUpdate { get; set; }
    public decimal CashOnHand { get; set; }
    public decimal CashAlertThreshold { get; set; } = 2000;
    public bool SpeedCompleteMode { get; set; } = true;
    public int EnergyLevel { get; set; } = 100;
    public int TotalPoints { get; set; }
    public int Level { get; set; } = 1;
    public DateTime? ShiftStartTime { get; set; }
    public string? LicenseImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public DriverPreferences? Preferences { get; set; }
    public ICollection<NotificationChannelPreference> NotificationChannelPreferences { get; set; } = new List<NotificationChannelPreference>();
    public ICollection<ActiveSession> ActiveSessions { get; set; } = new List<ActiveSession>();
    public ICollection<AccountDeletionRequest> AccountDeletionRequests { get; set; } = new List<AccountDeletionRequest>();
    public ICollection<UserConsent> UserConsents { get; set; } = new List<UserConsent>();
    public ICollection<DataDeletionRequest> DataDeletionRequests { get; set; } = new List<DataDeletionRequest>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<VoiceMemo> VoiceMemos { get; set; } = new List<VoiceMemo>();
    public ICollection<SyncQueue> SyncQueues { get; set; } = new List<SyncQueue>();

    // Phase 3: Customers & Partners
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<Partner> Partners { get; set; } = new List<Partner>();
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public ICollection<CallerIdNote> CallerIdNotes { get; set; } = new List<CallerIdNote>();
    public ICollection<BlockedCustomer> BlockedCustomers { get; set; } = new List<BlockedCustomer>();
}
