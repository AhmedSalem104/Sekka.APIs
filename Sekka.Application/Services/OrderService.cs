using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<OrderDto>> CreateAsync(Guid driverId, CreateOrderDto dto)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();

        if (!string.IsNullOrEmpty(dto.IdempotencyKey))
        {
            var existing = (await repo.ListAsync(new OrderByIdempotencyKeySpec(dto.IdempotencyKey))).FirstOrDefault();
            if (existing != null)
                return Result<OrderDto>.Conflict(ErrorMessages.InvalidIdempotencyKey);
        }

        var order = _mapper.Map<Order>(dto);
        order.DriverId = driverId;
        order.OrderNumber = await GenerateOrderNumber();
        order.Status = OrderStatus.Pending;
        order.AssignedAt = DateTime.UtcNow;

        await repo.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<Result<PagedResult<OrderListDto>>> GetOrdersAsync(Guid driverId, OrderFilterDto filter)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var spec = new DriverOrdersPagedSpec(driverId, filter);
        var items = await repo.ListAsync(spec);

        var countSpec = new DriverOrdersCountSpec(driverId, filter);
        var total = await repo.CountAsync(countSpec);

        var dtos = _mapper.Map<List<OrderListDto>>(items);
        var result = new PagedResult<OrderListDto>(dtos, total, filter.Page, filter.PageSize);

        return Result<PagedResult<OrderListDto>>.Success(result);
    }

    public async Task<Result<OrderDetailDto>> GetByIdAsync(Guid driverId, Guid orderId)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<OrderDetailDto>.NotFound(ErrorMessages.OrderNotFound);

        return Result<OrderDetailDto>.Success(_mapper.Map<OrderDetailDto>(order));
    }

    public async Task<Result<OrderDto>> UpdateAsync(Guid driverId, Guid orderId, UpdateOrderDto dto)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<OrderDto>.NotFound(ErrorMessages.OrderNotFound);

        if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
            return Result<OrderDto>.BadRequest(ErrorMessages.InvalidOrderStatus);

        if (dto.Description != null) order.Description = dto.Description;
        if (dto.Amount.HasValue) order.Amount = dto.Amount.Value;
        if (dto.PaymentMethod.HasValue) order.PaymentMethod = dto.PaymentMethod.Value;
        if (dto.Priority.HasValue) order.Priority = dto.Priority.Value;
        if (dto.DeliveryAddress != null) order.DeliveryAddress = dto.DeliveryAddress;
        if (dto.DeliveryLatitude.HasValue) order.DeliveryLatitude = dto.DeliveryLatitude.Value;
        if (dto.DeliveryLongitude.HasValue) order.DeliveryLongitude = dto.DeliveryLongitude.Value;
        if (dto.Notes != null) order.Notes = dto.Notes;
        if (dto.ItemCount.HasValue) order.ItemCount = dto.ItemCount.Value;
        if (dto.TimeWindowStart.HasValue) order.TimeWindowStart = dto.TimeWindowStart.Value;
        if (dto.TimeWindowEnd.HasValue) order.TimeWindowEnd = dto.TimeWindowEnd.Value;

        order.UpdatedAt = DateTime.UtcNow;
        repo.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<Result<bool>> DeleteAsync(Guid driverId, Guid orderId)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.OrderNotFound);

        order.IsDeleted = true;
        order.DeletedAt = DateTime.UtcNow;
        repo.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<OrderDto>> UpdateStatusAsync(Guid driverId, Guid orderId, UpdateStatusDto dto)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<OrderDto>.NotFound(ErrorMessages.OrderNotFound);

        order.Status = dto.Status;
        switch (dto.Status)
        {
            case OrderStatus.PickedUp:
                order.PickedUpAt = DateTime.UtcNow;
                break;
        }
        order.UpdatedAt = DateTime.UtcNow;
        repo.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<Result<OrderDto>> DeliverAsync(Guid driverId, Guid orderId, DeliverOrderDto dto)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<OrderDto>.NotFound(ErrorMessages.OrderNotFound);

        if (order.Status == OrderStatus.Delivered)
            return Result<OrderDto>.BadRequest(ErrorMessages.OrderAlreadyDelivered);

        order.Status = OrderStatus.Delivered;
        order.DeliveredAt = DateTime.UtcNow;
        order.ActualCollectedAmount = dto.ActualCollectedAmount;
        order.UpdatedAt = DateTime.UtcNow;
        repo.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<Result<DeliveryAttemptDto>> FailAsync(Guid driverId, Guid orderId, FailOrderDto dto)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<DeliveryAttemptDto>.NotFound(ErrorMessages.OrderNotFound);

        order.Status = OrderStatus.Failed;
        order.FailedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        repo.Update(order);

        var attemptRepo = _unitOfWork.GetRepository<DeliveryAttempt, Guid>();
        var attemptCount = await attemptRepo.CountAsync(new DeliveryAttemptsByOrderSpec(orderId));

        var attempt = new DeliveryAttempt
        {
            OrderId = orderId,
            AttemptNumber = attemptCount + 1,
            Status = dto.Reason,
            Reason = dto.ReasonText,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            AutoMessageSent = dto.SendAutoMessage,
            Timestamp = DateTime.UtcNow
        };

        await attemptRepo.AddAsync(attempt);
        await _unitOfWork.SaveChangesAsync();

        return Result<DeliveryAttemptDto>.Success(_mapper.Map<DeliveryAttemptDto>(attempt));
    }

    public async Task<Result<OrderDto>> PartialDeliverAsync(Guid driverId, Guid orderId, PartialDeliveryDto dto)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<OrderDto>.NotFound(ErrorMessages.OrderNotFound);

        order.Status = OrderStatus.PartialDelivery;
        order.ActualCollectedAmount = dto.ActualAmount;
        order.ReturnReason = dto.ReturnReason;
        order.PartialDeliveryNote = dto.Notes;
        order.DeliveredAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        repo.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<Result<OrderPhotoDto>> UploadPhotoAsync(Guid driverId, Guid orderId, Stream fileStream, string fileName, int photoType)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<OrderPhotoDto>.NotFound(ErrorMessages.OrderNotFound);

        var photoUrl = $"/uploads/orders/{orderId}/{Guid.NewGuid()}{Path.GetExtension(fileName)}";

        var photo = new OrderPhoto
        {
            OrderId = orderId,
            PhotoUrl = photoUrl,
            PhotoType = (PhotoType)photoType,
            TakenAt = DateTime.UtcNow
        };

        var photoRepo = _unitOfWork.GetRepository<OrderPhoto, Guid>();
        await photoRepo.AddAsync(photo);
        await _unitOfWork.SaveChangesAsync();

        return Result<OrderPhotoDto>.Success(_mapper.Map<OrderPhotoDto>(photo));
    }

    private Task<string> GenerateOrderNumber()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var seq = Guid.NewGuid().ToString("N")[..6].ToUpper();
        return Task.FromResult($"ORD-{date}-{seq}");
    }
}

// ── Specifications ──

internal class OrderByIdempotencyKeySpec : BaseSpecification<Order>
{
    public OrderByIdempotencyKeySpec(string key)
    {
        SetCriteria(o => o.IdempotencyKey == key);
    }
}

internal class DriverOrdersPagedSpec : BaseSpecification<Order>
{
    public DriverOrdersPagedSpec(Guid driverId, OrderFilterDto filter)
    {
        if (filter.Status.HasValue)
            SetCriteria(o => o.DriverId == driverId && o.Status == filter.Status.Value);
        else
            SetCriteria(o => o.DriverId == driverId);

        SetOrderByDescending(o => o.CreatedAt);
        ApplyPaging((filter.Page - 1) * filter.PageSize, filter.PageSize);
    }
}

internal class DriverOrdersCountSpec : BaseSpecification<Order>
{
    public DriverOrdersCountSpec(Guid driverId, OrderFilterDto filter)
    {
        if (filter.Status.HasValue)
            SetCriteria(o => o.DriverId == driverId && o.Status == filter.Status.Value);
        else
            SetCriteria(o => o.DriverId == driverId);
    }
}

internal class DeliveryAttemptsByOrderSpec : BaseSpecification<DeliveryAttempt>
{
    public DeliveryAttemptsByOrderSpec(Guid orderId)
    {
        SetCriteria(a => a.OrderId == orderId);
    }
}

internal class OrdersByDatePrefixSpec : BaseSpecification<Order>
{
    public OrdersByDatePrefixSpec(string datePrefix)
    {
        SetCriteria(o => o.OrderNumber.StartsWith($"ORD-{datePrefix}"));
    }
}
