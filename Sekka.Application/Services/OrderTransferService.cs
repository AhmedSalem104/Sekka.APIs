using AutoMapper;
using Microsoft.AspNetCore.Identity;
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

public class OrderTransferService : IOrderTransferService
{
    private const int TransferTimeoutMinutes = 30;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<Driver> _userManager;
    private readonly INotificationService _notificationService;
    private readonly ILogger<OrderTransferService> _logger;

    public OrderTransferService(IUnitOfWork unitOfWork, IMapper mapper,
        UserManager<Driver> userManager, INotificationService notificationService,
        ILogger<OrderTransferService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<OrderTransferResponseDto>> TransferAsync(Guid driverId, Guid orderId, TransferOrderDto dto)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<OrderTransferResponseDto>.NotFound(ErrorMessages.OrderNotFound);

        // Reject if order is already pending transfer
        if (order.Status == OrderStatus.PendingTransfer)
            return Result<OrderTransferResponseDto>.BadRequest("الأوردر في انتظار رد على تحويل سابق");

        // Check no existing pending transfer for this order
        var transferRepo = _unitOfWork.GetRepository<OrderTransferLog, Guid>();
        var existingTransfers = await transferRepo.ListAsync(new PendingTransferByOrderSpec(orderId));
        if (existingTransfers.Any())
            return Result<OrderTransferResponseDto>.BadRequest("يوجد طلب تحويل معلّق لهذا الأوردر بالفعل");

        var transfer = new OrderTransferLog
        {
            OrderId = orderId,
            FromDriverId = driverId,
            ToDriverId = dto.ToDriverId,
            TransferReason = dto.Reason,
            DeepLinkToken = Guid.NewGuid().ToString("N")[..16],
            Status = TransferStatus.Pending,
            PreviousOrderStatus = order.Status,
            TransferredAt = DateTime.UtcNow
        };

        await transferRepo.AddAsync(transfer);

        // Lock the order if transfer is to a specific driver
        if (dto.ToDriverId.HasValue)
        {
            order.Status = OrderStatus.PendingTransfer;
            repo.Update(order);
        }

        await _unitOfWork.SaveChangesAsync();

        // Notify the receiving driver (if specified)
        if (dto.ToDriverId.HasValue)
        {
            await _notificationService.CreateAndPushAsync(dto.ToDriverId.Value, NotificationType.NewOrder,
                "طلب تحويل أوردر", $"تم إرسال طلب تحويل أوردر #{order.OrderNumber} إليك — لديك 30 دقيقة للرد",
                "TRANSFER_REQUEST", transfer.Id.ToString());
        }

        // Build response
        var fromDriver = await _userManager.FindByIdAsync(driverId.ToString());
        Driver? toDriver = null;
        if (dto.ToDriverId.HasValue)
            toDriver = await _userManager.FindByIdAsync(dto.ToDriverId.Value.ToString());

        var response = new OrderTransferResponseDto
        {
            Id = transfer.Id,
            OrderNumber = order.OrderNumber,
            FromDriverName = fromDriver?.Name ?? "غير معروف",
            ToDriverName = toDriver?.Name,
            DeepLinkToken = transfer.DeepLinkToken,
            Status = transfer.Status,
            TransferredAt = transfer.TransferredAt
        };

        return Result<OrderTransferResponseDto>.Success(response);
    }

    public async Task<Result<List<TransferRequestDto>>> GetIncomingAsync(Guid driverId)
    {
        var transferRepo = _unitOfWork.GetRepository<OrderTransferLog, Guid>();
        var transfers = await transferRepo.ListAsync(new PendingTransfersByToDriverSpec(driverId));

        var result = new List<TransferRequestDto>();
        var repo = _unitOfWork.GetRepository<Order, Guid>();

        foreach (var t in transfers)
        {
            // Check timeout — expire if over 30 minutes
            if (IsExpired(t))
            {
                await ExpireTransfer(t, repo);
                continue;
            }

            var fromDriver = await _userManager.FindByIdAsync(t.FromDriverId.ToString());
            var order = t.Order ?? await repo.GetByIdAsync(t.OrderId);

            result.Add(MapToTransferRequestDto(t, order, fromDriver?.Name, null));
        }

        if (transfers.Any(IsExpired))
            await _unitOfWork.SaveChangesAsync();

        return Result<List<TransferRequestDto>>.Success(result);
    }

    public async Task<Result<List<TransferRequestDto>>> GetOutgoingAsync(Guid driverId)
    {
        var transferRepo = _unitOfWork.GetRepository<OrderTransferLog, Guid>();
        var transfers = await transferRepo.ListAsync(new PendingTransfersByFromDriverSpec(driverId));

        var result = new List<TransferRequestDto>();
        var repo = _unitOfWork.GetRepository<Order, Guid>();

        foreach (var t in transfers)
        {
            // Check timeout
            if (IsExpired(t))
            {
                await ExpireTransfer(t, repo);
                continue;
            }

            Driver? toDriver = null;
            if (t.ToDriverId.HasValue)
                toDriver = await _userManager.FindByIdAsync(t.ToDriverId.Value.ToString());
            var order = t.Order ?? await repo.GetByIdAsync(t.OrderId);

            result.Add(MapToTransferRequestDto(t, order, null, toDriver?.Name));
        }

        if (transfers.Any(IsExpired))
            await _unitOfWork.SaveChangesAsync();

        return Result<List<TransferRequestDto>>.Success(result);
    }

    public async Task<Result<OrderTransferResponseDto>> AcceptAsync(Guid driverId, Guid transferId)
    {
        var transferRepo = _unitOfWork.GetRepository<OrderTransferLog, Guid>();
        var transfer = await transferRepo.GetByIdAsync(transferId);

        if (transfer == null)
            return Result<OrderTransferResponseDto>.NotFound(ErrorMessages.TransferNotFound);

        if (transfer.ToDriverId != driverId)
            return Result<OrderTransferResponseDto>.BadRequest("غير مصرح لك بقبول هذا التحويل");

        if (transfer.Status != TransferStatus.Pending)
            return Result<OrderTransferResponseDto>.BadRequest("طلب التحويل لم يعد معلّقاً");

        // Check timeout
        if (IsExpired(transfer))
        {
            var orderRepo2 = _unitOfWork.GetRepository<Order, Guid>();
            await ExpireTransfer(transfer, orderRepo2);
            await _unitOfWork.SaveChangesAsync();
            return Result<OrderTransferResponseDto>.BadRequest("انتهت مهلة طلب التحويل");
        }

        // Accept the transfer
        transfer.Status = TransferStatus.Accepted;
        transfer.AcceptedAt = DateTime.UtcNow;
        transferRepo.Update(transfer);

        // Move the order to the new driver
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(transfer.OrderId);
        if (order == null)
            return Result<OrderTransferResponseDto>.NotFound(ErrorMessages.OrderNotFound);

        order.DriverId = driverId;
        order.Status = OrderStatus.Pending;
        order.AssignedAt = DateTime.UtcNow;
        repo.Update(order);

        await _unitOfWork.SaveChangesAsync();

        // Notify the original driver
        await _notificationService.CreateAndPushAsync(transfer.FromDriverId, NotificationType.NewOrder,
            "تم قبول التحويل", $"زميلك قبل أوردر #{order.OrderNumber}",
            "TRANSFER_ACCEPTED", transfer.Id.ToString());

        var fromDriver = await _userManager.FindByIdAsync(transfer.FromDriverId.ToString());
        var toDriver = await _userManager.FindByIdAsync(driverId.ToString());

        var response = new OrderTransferResponseDto
        {
            Id = transfer.Id,
            OrderNumber = order.OrderNumber,
            FromDriverName = fromDriver?.Name ?? "غير معروف",
            ToDriverName = toDriver?.Name,
            DeepLinkToken = transfer.DeepLinkToken,
            Status = transfer.Status,
            TransferredAt = transfer.TransferredAt
        };

        return Result<OrderTransferResponseDto>.Success(response);
    }

    public async Task<Result<bool>> RejectAsync(Guid driverId, Guid transferId, string? reason)
    {
        var transferRepo = _unitOfWork.GetRepository<OrderTransferLog, Guid>();
        var transfer = await transferRepo.GetByIdAsync(transferId);

        if (transfer == null)
            return Result<bool>.NotFound(ErrorMessages.TransferNotFound);

        if (transfer.ToDriverId != driverId)
            return Result<bool>.BadRequest("غير مصرح لك برفض هذا التحويل");

        if (transfer.Status != TransferStatus.Pending)
            return Result<bool>.BadRequest("طلب التحويل لم يعد معلّقاً");

        // Reject the transfer
        transfer.Status = TransferStatus.Rejected;
        transferRepo.Update(transfer);

        // Restore the order status
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(transfer.OrderId);
        if (order != null && order.Status == OrderStatus.PendingTransfer)
        {
            order.Status = transfer.PreviousOrderStatus ?? OrderStatus.Accepted;
            repo.Update(order);
        }

        await _unitOfWork.SaveChangesAsync();

        // Notify the original driver
        var reasonText = string.IsNullOrEmpty(reason) ? "" : $" — السبب: {reason}";
        await _notificationService.CreateAndPushAsync(transfer.FromDriverId, NotificationType.NewOrder,
            "تم رفض التحويل", $"زميلك رفض أوردر #{order?.OrderNumber}{reasonText}",
            "TRANSFER_REJECTED", transfer.Id.ToString());

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> CancelAsync(Guid driverId, Guid transferId)
    {
        var transferRepo = _unitOfWork.GetRepository<OrderTransferLog, Guid>();
        var transfer = await transferRepo.GetByIdAsync(transferId);

        if (transfer == null)
            return Result<bool>.NotFound(ErrorMessages.TransferNotFound);

        if (transfer.FromDriverId != driverId)
            return Result<bool>.BadRequest("غير مصرح لك بإلغاء هذا التحويل");

        if (transfer.Status != TransferStatus.Pending)
            return Result<bool>.BadRequest("طلب التحويل لم يعد معلّقاً");

        // Cancel the transfer
        transfer.Status = TransferStatus.Rejected;
        transferRepo.Update(transfer);

        // Restore the order status
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(transfer.OrderId);
        if (order != null && order.Status == OrderStatus.PendingTransfer)
        {
            order.Status = transfer.PreviousOrderStatus ?? OrderStatus.Accepted;
            repo.Update(order);
        }

        await _unitOfWork.SaveChangesAsync();

        // Notify the receiving driver
        if (transfer.ToDriverId.HasValue)
        {
            await _notificationService.CreateAndPushAsync(transfer.ToDriverId.Value, NotificationType.NewOrder,
                "تم إلغاء طلب التحويل", $"السواق ألغى طلب تحويل أوردر #{order?.OrderNumber}",
                "TRANSFER_CANCELLED", transfer.Id.ToString());
        }

        return Result<bool>.Success(true);
    }

    #region Private Helpers

    private static bool IsExpired(OrderTransferLog transfer)
        => DateTime.UtcNow > transfer.TransferredAt.AddMinutes(TransferTimeoutMinutes);

    private async Task ExpireTransfer(OrderTransferLog transfer, IGenericRepository<Order, Guid> orderRepo)
    {
        transfer.Status = TransferStatus.Expired;
        var transferRepo = _unitOfWork.GetRepository<OrderTransferLog, Guid>();
        transferRepo.Update(transfer);

        var order = transfer.Order ?? await orderRepo.GetByIdAsync(transfer.OrderId);
        if (order != null && order.Status == OrderStatus.PendingTransfer)
        {
            order.Status = transfer.PreviousOrderStatus ?? OrderStatus.Accepted;
            orderRepo.Update(order);
        }

        // Notify both drivers
        await _notificationService.CreateAndPushAsync(transfer.FromDriverId, NotificationType.NewOrder,
            "انتهت مهلة التحويل", $"انتهت مهلة طلب تحويل أوردر #{order?.OrderNumber}",
            "TRANSFER_EXPIRED", transfer.Id.ToString());

        if (transfer.ToDriverId.HasValue)
        {
            await _notificationService.CreateAndPushAsync(transfer.ToDriverId.Value, NotificationType.NewOrder,
                "انتهت مهلة التحويل", $"انتهت مهلة طلب تحويل أوردر #{order?.OrderNumber}",
                "TRANSFER_EXPIRED", transfer.Id.ToString());
        }
    }

    private static TransferRequestDto MapToTransferRequestDto(OrderTransferLog t, Order? order, string? fromName, string? toName)
    {
        var expiresAt = t.TransferredAt.AddMinutes(TransferTimeoutMinutes);
        var remaining = (int)Math.Max(0, (expiresAt - DateTime.UtcNow).TotalMinutes);

        return new TransferRequestDto
        {
            Id = t.Id,
            OrderId = t.OrderId,
            OrderNumber = order?.OrderNumber ?? "",
            CustomerName = order?.CustomerName,
            DeliveryAddress = order?.DeliveryAddress,
            Amount = order?.Amount ?? 0,
            FromDriverName = fromName ?? "غير معروف",
            ToDriverName = toName,
            TransferReason = t.TransferReason,
            Status = t.Status,
            TransferredAt = t.TransferredAt,
            ExpiresAt = expiresAt,
            RemainingMinutes = remaining
        };
    }

    #endregion
}

#region Specifications

internal class PendingTransfersByToDriverSpec : BaseSpecification<OrderTransferLog>
{
    public PendingTransfersByToDriverSpec(Guid driverId)
    {
        SetCriteria(t => t.ToDriverId == driverId && t.Status == TransferStatus.Pending);
        SetOrderByDescending(t => t.TransferredAt);
        AddInclude(t => t.Order);
    }
}

internal class PendingTransfersByFromDriverSpec : BaseSpecification<OrderTransferLog>
{
    public PendingTransfersByFromDriverSpec(Guid driverId)
    {
        SetCriteria(t => t.FromDriverId == driverId && t.Status == TransferStatus.Pending);
        SetOrderByDescending(t => t.TransferredAt);
        AddInclude(t => t.Order);
    }
}

internal class PendingTransferByOrderSpec : BaseSpecification<OrderTransferLog>
{
    public PendingTransferByOrderSpec(Guid orderId)
    {
        SetCriteria(t => t.OrderId == orderId && t.Status == TransferStatus.Pending);
    }
}

#endregion
