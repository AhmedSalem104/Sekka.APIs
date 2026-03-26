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
    }
}
