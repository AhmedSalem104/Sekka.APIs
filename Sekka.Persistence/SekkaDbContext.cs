using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sekka.Persistence.Entities;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence;

public class SekkaDbContext : IdentityDbContext<Driver, IdentityRole<Guid>, Guid>
{
    public SekkaDbContext(DbContextOptions<SekkaDbContext> options) : base(options)
    {
    }

    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<DriverPreferences> DriverPreferences => Set<DriverPreferences>();
    public DbSet<NotificationChannelPreference> NotificationChannelPreferences => Set<NotificationChannelPreference>();
    public DbSet<AccountDeletionRequest> AccountDeletionRequests => Set<AccountDeletionRequest>();
    public DbSet<ActiveSession> ActiveSessions => Set<ActiveSession>();
    public DbSet<UserConsent> UserConsents => Set<UserConsent>();
    public DbSet<DataDeletionRequest> DataDeletionRequests => Set<DataDeletionRequest>();

    // Phase 2: Orders & Delivery
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<DeliveryAttempt> DeliveryAttempts => Set<DeliveryAttempt>();
    public DbSet<OrderPhoto> OrderPhotos => Set<OrderPhoto>();
    public DbSet<AddressSwapLog> AddressSwapLogs => Set<AddressSwapLog>();
    public DbSet<CancellationLog> CancellationLogs => Set<CancellationLog>();
    public DbSet<OrderSourceTag> OrderSourceTags => Set<OrderSourceTag>();
    public DbSet<WaitingTimer> WaitingTimers => Set<WaitingTimer>();
    public DbSet<OrderTransferLog> OrderTransferLogs => Set<OrderTransferLog>();
    public DbSet<VoiceMemo> VoiceMemos => Set<VoiceMemo>();
    public DbSet<SyncQueue> SyncQueues => Set<SyncQueue>();
    public DbSet<TrackingLink> TrackingLinks => Set<TrackingLink>();
    public DbSet<DeliveryTimeSlot> DeliveryTimeSlots => Set<DeliveryTimeSlot>();

    // Phase 3: Customers & Partners
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<CallerIdNote> CallerIdNotes => Set<CallerIdNote>();
    public DbSet<BlockedCustomer> BlockedCustomers => Set<BlockedCustomer>();
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<PickupPoint> PickupPoints => Set<PickupPoint>();
    public DbSet<CommunityBlacklist> CommunityBlacklist => Set<CommunityBlacklist>();

    // Phase 4: Financial
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<Settlement> Settlements => Set<Settlement>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<PaymentRequest> PaymentRequests => Set<PaymentRequest>();
    public DbSet<DailyStats> DailyStats => Set<DailyStats>();
    public DbSet<OrderDispute> OrderDisputes => Set<OrderDispute>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<RefundRequest> RefundRequests => Set<RefundRequest>();
    public DbSet<SurgePricingRule> SurgePricingRules => Set<SurgePricingRule>();

    // Phase 5: Communication
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<DeviceToken> DeviceTokens => Set<DeviceToken>();
    public DbSet<QuickMessageTemplate> QuickMessageTemplates => Set<QuickMessageTemplate>();
    public DbSet<SOSLog> SOSLogs => Set<SOSLog>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();

    // Phase 6: Location & Vehicles
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<ParkingSpot> ParkingSpots => Set<ParkingSpot>();
    public DbSet<LocationHistory> LocationHistories => Set<LocationHistory>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();
    public DbSet<BreakLog> BreakLogs => Set<BreakLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SekkaDbContext).Assembly);

        // Global query filter for soft-deletable entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(SoftDeletableEntity<Guid>).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(SekkaDbContext)
                    .GetMethod(nameof(ApplySoftDeleteFilter),
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(null, new object[] { modelBuilder });
            }
        }
    }

    private static void ApplySoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : SoftDeletableEntity<Guid>
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }
}
