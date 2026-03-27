using AutoMapper;
using Sekka.Core.DTOs.Order;
using Sekka.Core.DTOs.Route;
using Sekka.Core.DTOs.Sync;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        // ── Phase 1: Auth & Identity ──
        CreateMap<DriverPreferences, Core.DTOs.Settings.DriverPreferencesDto>();
        CreateMap<NotificationChannelPreference, Core.DTOs.Settings.NotificationChannelPrefDto>();
        CreateMap<ActiveSession, Core.DTOs.Account.ActiveSessionDto>();
        CreateMap<UserConsent, Core.DTOs.Privacy.ConsentDto>();

        // ── Orders ──
        CreateMap<Order, OrderDto>();
        CreateMap<Order, OrderListDto>();
        CreateMap<Order, OrderDetailDto>();
        CreateMap<Order, AdminOrderDto>();
        CreateMap<CreateOrderDto, Order>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.OrderNumber, opt => opt.Ignore())
            .ForMember(d => d.DriverId, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.CommissionAmount, opt => opt.Ignore());

        // ── Order Sub-entities ──
        CreateMap<DeliveryAttempt, DeliveryAttemptDto>();
        CreateMap<OrderPhoto, OrderPhotoDto>();
        CreateMap<AddressSwapLog, AddressSwapLogDto>();
        CreateMap<CancellationLog, CancellationLogDto>()
            .ForMember(d => d.Reason, opt => opt.MapFrom(s => s.CancellationReason));
        CreateMap<OrderSourceTag, OrderSourceTagDto>();
        CreateMap<WaitingTimer, WaitingTimerDto>();
        CreateMap<OrderTransferLog, OrderTransferResponseDto>();
        CreateMap<VoiceMemo, VoiceMemoDto>();
        CreateMap<DeliveryTimeSlot, TimeSlotDto>()
            .ForMember(d => d.AvailableSlots, opt => opt.MapFrom(s => s.MaxCapacity - s.CurrentBookings))
            .ForMember(d => d.IsAvailable, opt => opt.MapFrom(s => s.IsActive && s.CurrentBookings < s.MaxCapacity));

        // ── Recurring ──
        CreateMap<Order, RecurringOrderDto>();

        // ── Sync ──
        CreateMap<SyncQueue, SyncChangeDto>()
            .ForMember(d => d.LocalTimestamp, opt => opt.MapFrom(s => s.CreatedAt));

        // ── Phase 3: Customers & Partners ──
        CreateMap<Customer, Core.DTOs.Customer.CustomerDto>();
        CreateMap<Customer, Core.DTOs.Customer.CustomerListDto>();
        CreateMap<Customer, Core.DTOs.Customer.CustomerDetailDto>();
        CreateMap<Address, Core.DTOs.Customer.AddressDto>();
        CreateMap<Core.DTOs.Customer.SaveAddressDto, Address>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.DriverId, opt => opt.Ignore());
        CreateMap<Rating, Core.DTOs.Customer.RatingDto>();
        CreateMap<CallerIdNote, Core.DTOs.Customer.CallerIdNoteDto>();
        CreateMap<CallerIdNote, Core.DTOs.Customer.CallerIdDto>();
        CreateMap<BlockedCustomer, Core.DTOs.Admin.BlacklistEntryDto>();
        CreateMap<Partner, Core.DTOs.Partner.PartnerDto>();
        CreateMap<Core.DTOs.Partner.CreatePartnerDto, Partner>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.DriverId, opt => opt.Ignore());
        CreateMap<Partner, Core.DTOs.Partner.PartnerVerificationDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.VerificationStatus))
            .ForMember(d => d.DocumentUrl, opt => opt.MapFrom(s => s.VerificationDocumentUrl))
            .ForMember(d => d.Note, opt => opt.MapFrom(s => s.VerificationNote));
        CreateMap<PickupPoint, Core.DTOs.Partner.PickupPointDto>();
        CreateMap<Core.DTOs.Partner.CreatePickupPointDto, PickupPoint>()
            .ForMember(d => d.Id, opt => opt.Ignore());
        CreateMap<CommunityBlacklist, Core.DTOs.Admin.BlacklistEntryDto>();

        // ── Phase 4: Financial ──
        CreateMap<WalletTransaction, Core.DTOs.Wallet.WalletTransactionDto>();
        CreateMap<Settlement, Core.DTOs.Settlement.SettlementDto>();
        CreateMap<Core.DTOs.Settlement.CreateSettlementDto, Settlement>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.DriverId, opt => opt.Ignore());
        CreateMap<Expense, Core.DTOs.Wallet.ExpenseDto>();
        CreateMap<Core.DTOs.Wallet.CreateExpenseDto, Expense>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.DriverId, opt => opt.Ignore());
        CreateMap<PaymentRequest, Core.DTOs.Wallet.PaymentRequestDto>();
        CreateMap<DailyStats, Core.DTOs.Analytics.DailyStatsDto>();
        CreateMap<OrderDispute, Core.DTOs.Wallet.DisputeDto>();
        CreateMap<Invoice, Core.DTOs.Wallet.InvoiceDto>();
        CreateMap<InvoiceItem, Core.DTOs.Wallet.InvoiceItemDto>();
        CreateMap<RefundRequest, Core.DTOs.Wallet.RefundDto>();
        CreateMap<SurgePricingRule, object>(); // placeholder

        // ── Phase 5: Communication ──
        CreateMap<Notification, Core.DTOs.Communication.NotificationDto>();
        CreateMap<SOSLog, Core.DTOs.Communication.SOSLogDto>();
        CreateMap<QuickMessageTemplate, Core.DTOs.Communication.MessageTemplateDto>();
        CreateMap<Conversation, Core.DTOs.Communication.ConversationDto>();
        CreateMap<ChatMessage, Core.DTOs.Communication.ChatMessageDto>();
        CreateMap<DeviceToken, object>(); // placeholder

        // ── Phase 6: Location & Vehicles ──
        CreateMap<Vehicle, Core.DTOs.Vehicle.VehicleDto>();
        CreateMap<Core.DTOs.Vehicle.CreateVehicleDto, Vehicle>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.DriverId, opt => opt.Ignore());
        CreateMap<MaintenanceRecord, Core.DTOs.Vehicle.MaintenanceRecordDto>();
        CreateMap<ParkingSpot, Core.DTOs.Vehicle.ParkingSpotDto>();
        CreateMap<BreakLog, Core.DTOs.Vehicle.BreakLogDto>();
        CreateMap<Route, Core.DTOs.Route.RouteDto>();

        // ── Phase 7: Intelligence ──
        CreateMap<InterestCategory, Core.DTOs.Intelligence.InterestCategorySummaryDto>();
        CreateMap<CustomerInterest, Core.DTOs.Intelligence.CustomerInterestDto>();
        CreateMap<BehaviorPattern, Core.DTOs.Intelligence.BehaviorPatternDto>();
        CreateMap<CustomerRecommendation, Core.DTOs.Intelligence.CustomerRecommendationDto>();
        CreateMap<CustomerSegment, Core.DTOs.Admin.AdminSegmentDto>();
        CreateMap<CustomerSegment, Core.DTOs.Admin.AdminSegmentDetailDto>();
        CreateMap<CustomerSegmentMember, Core.DTOs.Admin.SegmentMemberDto>();
        CreateMap<CampaignTarget, Core.DTOs.Admin.AdminCampaignDto>();
        CreateMap<CampaignTarget, Core.DTOs.Admin.AdminCampaignDetailDto>();

        // ── Phase 8: Admin & System ──
        CreateMap<Region, Core.DTOs.System.RegionDto>();
        CreateMap<WebhookConfig, Core.DTOs.System.WebhookConfigDto>();
        CreateMap<WebhookLog, Core.DTOs.System.WebhookLogDto>();
        CreateMap<AppVersion, Core.DTOs.System.AppVersionDto>();
        CreateMap<FeatureFlag, Core.DTOs.System.FeatureFlagDto>();
        CreateMap<MaintenanceWindow, Core.DTOs.System.MaintenanceWindowDto>();
        CreateMap<SystemNotice, Core.DTOs.System.SystemNoticeDto>();
        CreateMap<AuditLog, Core.DTOs.System.AuditLogDto>();

        // ── Phase 9: Social & Extras ──
        CreateMap<SubscriptionPlan, object>(); // placeholder
        CreateMap<Subscription, object>(); // placeholder
        CreateMap<Challenge, Core.DTOs.Social.ChallengeDto>();
        CreateMap<DriverAchievement, Core.DTOs.Social.DriverAchievementDto>();
        CreateMap<Referral, Core.DTOs.Social.ReferralDto>();
        CreateMap<SavingsCircle, Core.DTOs.Social.CircleDto>();
        CreateMap<SavingsCircleMember, Core.DTOs.Social.CircleMemberDto>();
        CreateMap<SavingsCirclePayment, Core.DTOs.Social.CirclePaymentDto>();
        CreateMap<FieldAssistanceRequest, Core.DTOs.Social.HelpRequestDto>();
        CreateMap<RoadReport, Core.DTOs.Social.RoadReportDto>();
    }
}
