using Microsoft.AspNetCore.Identity;
using Sekka.Core.Enums;

namespace Sekka.Persistence.Entities;

public class Driver : IdentityUser<Guid>
{
    public string Name { get; set; } = null!;
    public string? ProfileImageUrl { get; set; }
    public VehicleType? VehicleType { get; set; }
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

    // Phase 4: Financial
    public ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
    public ICollection<Settlement> Settlements { get; set; } = new List<Settlement>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<PaymentRequest> PaymentRequests { get; set; } = new List<PaymentRequest>();
    public ICollection<DailyStats> DailyStats { get; set; } = new List<DailyStats>();
    public ICollection<OrderDispute> OrderDisputes { get; set; } = new List<OrderDispute>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<RefundRequest> RefundRequests { get; set; } = new List<RefundRequest>();

    // Phase 5: Communication
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<DeviceToken> DeviceTokens { get; set; } = new List<DeviceToken>();
    public ICollection<QuickMessageTemplate> MessageTemplates { get; set; } = new List<QuickMessageTemplate>();
    public ICollection<SOSLog> SOSLogs { get; set; } = new List<SOSLog>();
    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    // Phase 6: Location & Vehicles
    public ICollection<Route> Routes { get; set; } = new List<Route>();
    public ICollection<ParkingSpot> ParkingSpots { get; set; } = new List<ParkingSpot>();
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public ICollection<BreakLog> BreakLogs { get; set; } = new List<BreakLog>();

    // Phase 7: Intelligence
    public ICollection<CustomerInterest> CustomerInterestsData { get; set; } = new List<CustomerInterest>();
    public ICollection<BehaviorPattern> BehaviorPatterns { get; set; } = new List<BehaviorPattern>();

    // Phase 9: Social & Extras
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<DriverAchievement> Achievements { get; set; } = new List<DriverAchievement>();
    public ICollection<Referral> ReferralsSent { get; set; } = new List<Referral>();
    public ICollection<Referral> ReferralsReceived { get; set; } = new List<Referral>();
    public ICollection<SavingsCircle> SavingsCirclesCreated { get; set; } = new List<SavingsCircle>();
    public ICollection<SavingsCircleMember> SavingsCircleMemberships { get; set; } = new List<SavingsCircleMember>();
    public ICollection<FieldAssistanceRequest> FieldAssistanceRequestsSent { get; set; } = new List<FieldAssistanceRequest>();
    public ICollection<RoadReport> RoadReports { get; set; } = new List<RoadReport>();
}
