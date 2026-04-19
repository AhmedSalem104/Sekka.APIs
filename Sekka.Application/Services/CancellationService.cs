using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class CancellationService : ICancellationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CancellationService> _logger;

    public CancellationService(IUnitOfWork unitOfWork, IMapper mapper,
        INotificationService notificationService, ILogger<CancellationService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<CancellationLogDto>> CancelOrderAsync(Guid driverId, Guid orderId, CancelOrderDto dto)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<CancellationLogDto>.NotFound(ErrorMessages.OrderNotFound);

        if (order.Status == OrderStatus.Delivered)
            return Result<CancellationLogDto>.BadRequest(ErrorMessages.CannotCancelDelivered);

        if (order.Status == OrderStatus.Cancelled)
            return Result<CancellationLogDto>.BadRequest(ErrorMessages.OrderAlreadyCancelled);

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;
        repo.Update(order);

        var log = new CancellationLog
        {
            OrderId = orderId,
            CancellationReason = dto.CancellationReason,
            ReasonText = dto.ReasonText,
            LossAmount = dto.LossAmount,
            DistanceTravelledKm = dto.DistanceTravelledKm,
            FuelCostLost = dto.FuelCostLost,
            TransferredToDriverId = dto.TransferToDriverId,
            CancelledAt = DateTime.UtcNow
        };

        var logRepo = _unitOfWork.GetRepository<CancellationLog, Guid>();
        await logRepo.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();

        await _notificationService.CreateAndPushAsync(driverId, NotificationType.NewOrder,
            "تم إلغاء الأوردر", $"أوردر #{order.OrderNumber} تم إلغاؤه",
            "ORDER", orderId.ToString());

        return Result<CancellationLogDto>.Success(_mapper.Map<CancellationLogDto>(log));
    }
}
