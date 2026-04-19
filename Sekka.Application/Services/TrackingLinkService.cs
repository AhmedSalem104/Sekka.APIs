using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class TrackingLinkService : ITrackingLinkService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TrackingLinkService> _logger;

    public TrackingLinkService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TrackingLinkService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TrackingPageDto>> GetTrackingPageAsync(string trackingCode)
    {
        _logger.LogInformation("Tracking page requested for code {TrackingCode}", trackingCode);

        var repo = _unitOfWork.GetRepository<TrackingLink, Guid>();
        var spec = new TrackingLinkByCodeSpec(trackingCode);
        var links = await repo.ListAsync(spec);
        var link = links.FirstOrDefault();

        if (link is null)
            return Result<TrackingPageDto>.NotFound("رابط التتبع غير موجود");

        if (!link.IsActive || link.ExpiresAt < DateTime.UtcNow)
            return Result<TrackingPageDto>.BadRequest("رابط التتبع منتهي الصلاحية");

        // Increment view count
        link.ViewCount++;
        repo.Update(link);
        await _unitOfWork.SaveChangesAsync();

        // Get the associated order
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await orderRepo.GetByIdAsync(link.OrderId);

        if (order is null)
            return Result<TrackingPageDto>.NotFound(ErrorMessages.ItemNotFound);

        // Get driver location
        var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();
        var driver = await driverRepo.GetByIdAsync(order.DriverId);

        // Build timeline
        var timeline = BuildTrackingTimeline(order);

        var dto = new TrackingPageDto
        {
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            DriverName = driver?.Name,
            PickupAddress = order.PickupAddress,
            DeliveryAddress = order.DeliveryAddress,
            DriverLatitude = driver?.IsOnline == true ? driver.LastKnownLatitude : null,
            DriverLongitude = driver?.IsOnline == true ? driver.LastKnownLongitude : null,
            Timeline = timeline
        };

        return Result<TrackingPageDto>.Success(dto);
    }

    public async Task<Result<ShareLinkDto>> CreateShareLinkAsync(Guid driverId, Guid orderId, int? ttlMinutes = null)
    {
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await orderRepo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<ShareLinkDto>.NotFound(ErrorMessages.OrderNotFound);

        var ttl = ttlMinutes ?? 60;
        var trackingCode = Guid.NewGuid().ToString("N")[..12];

        var link = new TrackingLink
        {
            OrderId = orderId,
            TrackingCode = trackingCode,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddMinutes(ttl)
        };

        var repo = _unitOfWork.GetRepository<TrackingLink, Guid>();

        // Deactivate existing links for this order
        var existingSpec = new TrackingLinkByOrderSpec(orderId);
        var existingLinks = await repo.ListAsync(existingSpec);
        foreach (var existing in existingLinks)
        {
            existing.IsActive = false;
            repo.Update(existing);
        }

        await repo.AddAsync(link);
        await _unitOfWork.SaveChangesAsync();

        var shareUrl = $"https://sekka.app/o/{trackingCode}";
        var customerName = order.CustomerName ?? "العميل";
        var messageTemplate = $"أوردر #{order.OrderNumber} لـ {customerName} — {order.DeliveryAddress}\nالمبلغ: {order.Amount} جنيه\nتتبع الأوردر: {shareUrl}";

        var dto = new ShareLinkDto
        {
            ShareToken = trackingCode,
            ShareUrl = shareUrl,
            ExpiresAt = link.ExpiresAt,
            MessageTemplate = messageTemplate
        };

        _logger.LogInformation("Share link created for order {OrderId}: {TrackingCode}", orderId, trackingCode);

        return Result<ShareLinkDto>.Success(dto);
    }

    private static List<TrackingTimelineDto> BuildTrackingTimeline(Order order)
    {
        var timeline = new List<TrackingTimelineDto>();
        var statusOrder = new[] { OrderStatus.Pending, OrderStatus.Accepted, OrderStatus.PickedUp, OrderStatus.InTransit, OrderStatus.Delivered };

        foreach (var status in statusOrder)
        {
            var timestamp = status switch
            {
                OrderStatus.Pending => order.CreatedAt,
                OrderStatus.Accepted => order.AssignedAt ?? default,
                OrderStatus.PickedUp => order.PickedUpAt ?? default,
                OrderStatus.InTransit => order.ArrivedAt ?? default,
                OrderStatus.Delivered => order.DeliveredAt ?? default,
                _ => default
            };

            var description = status switch
            {
                OrderStatus.Pending => "تم إنشاء الطلب",
                OrderStatus.Accepted => "تم قبول الطلب",
                OrderStatus.PickedUp => "تم استلام الطلب",
                OrderStatus.InTransit => "الطلب في الطريق",
                OrderStatus.Delivered => "تم التسليم",
                _ => status.ToString()
            };

            var isCompleted = order.Status >= status;
            var isCurrent = order.Status == status;

            timeline.Add(new TrackingTimelineDto
            {
                Status = status,
                Timestamp = timestamp != default ? timestamp : DateTime.UtcNow,
                Description = description,
                IsCompleted = isCompleted,
                IsCurrent = isCurrent
            });
        }

        return timeline;
    }
}

// ── Specifications ──

internal class TrackingLinkByCodeSpec : BaseSpecification<TrackingLink>
{
    public TrackingLinkByCodeSpec(string trackingCode)
    {
        SetCriteria(t => t.TrackingCode == trackingCode);
    }
}

internal class TrackingLinkByOrderSpec : BaseSpecification<TrackingLink>
{
    public TrackingLinkByOrderSpec(Guid orderId)
    {
        SetCriteria(t => t.OrderId == orderId && t.IsActive);
    }
}
