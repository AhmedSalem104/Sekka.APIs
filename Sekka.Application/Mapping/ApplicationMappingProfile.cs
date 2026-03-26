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
    }
}
