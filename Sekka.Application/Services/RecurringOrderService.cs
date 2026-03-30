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

public class RecurringOrderService : IRecurringOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RecurringOrderService> _logger;

    public RecurringOrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RecurringOrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<RecurringOrderDto>>> GetRecurringOrdersAsync(Guid driverId)
    {
        _logger.LogInformation("Recurring orders requested by driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var spec = new RecurringOrdersByDriverSpec(driverId);
        var orders = await repo.ListAsync(spec);

        var dtos = orders.Select(o => MapToRecurringDto(o)).ToList();
        return Result<List<RecurringOrderDto>>.Success(dtos);
    }

    public async Task<Result<RecurringOrderDto>> CreateRecurringOrderAsync(Guid driverId, CreateRecurringOrderDto dto)
    {
        _logger.LogInformation("Create recurring order requested by driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<Order, Guid>();

        var order = new Order
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            OrderNumber = $"REC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
            CustomerName = dto.CustomerName,
            CustomerPhone = dto.CustomerPhone != null ? EgyptianPhoneHelper.Normalize(dto.CustomerPhone) : null,
            PartnerId = dto.PartnerId,
            Description = dto.Description,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            Priority = dto.Priority,
            PickupAddress = dto.PickupAddress,
            PickupLatitude = dto.PickupLatitude,
            PickupLongitude = dto.PickupLongitude,
            DeliveryAddress = dto.DeliveryAddress,
            DeliveryLatitude = dto.DeliveryLatitude,
            DeliveryLongitude = dto.DeliveryLongitude,
            Notes = dto.Notes,
            ItemCount = dto.ItemCount,
            Status = OrderStatus.Pending,
            SourceType = OrderSourceType.Recurring,
            IsRecurring = true,
            RecurrencePattern = dto.RecurrencePattern,
            ScheduledDate = dto.StartDate
        };

        await repo.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Recurring order {OrderId} created by driver {DriverId}", order.Id, driverId);

        return Result<RecurringOrderDto>.Success(MapToRecurringDto(order));
    }

    public async Task<Result<RecurringOrderDto>> UpdateRecurringOrderAsync(Guid driverId, Guid orderId, UpdateRecurringOrderDto dto)
    {
        _logger.LogInformation("Update recurring order {OrderId} requested by driver {DriverId}", orderId, driverId);

        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order is null || order.DriverId != driverId || !order.IsRecurring)
            return Result<RecurringOrderDto>.NotFound(ErrorMessages.ItemNotFound);

        if (dto.RecurrencePattern != null)
            order.RecurrencePattern = dto.RecurrencePattern;

        order.UpdatedAt = DateTime.UtcNow;
        repo.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return Result<RecurringOrderDto>.Success(MapToRecurringDto(order));
    }

    public async Task<Result<bool>> PauseAsync(Guid driverId, Guid orderId)
    {
        _logger.LogInformation("Pause recurring order {OrderId} requested by driver {DriverId}", orderId, driverId);

        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order is null || order.DriverId != driverId || !order.IsRecurring)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        order.Status = OrderStatus.Postponed;
        order.UpdatedAt = DateTime.UtcNow;
        repo.Update(order);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Recurring order {OrderId} paused", orderId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ResumeAsync(Guid driverId, Guid orderId)
    {
        _logger.LogInformation("Resume recurring order {OrderId} requested by driver {DriverId}", orderId, driverId);

        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order is null || order.DriverId != driverId || !order.IsRecurring)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        order.Status = OrderStatus.Pending;
        order.UpdatedAt = DateTime.UtcNow;
        repo.Update(order);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Recurring order {OrderId} resumed", orderId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid driverId, Guid orderId)
    {
        _logger.LogInformation("Delete recurring order {OrderId} requested by driver {DriverId}", orderId, driverId);

        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order is null || order.DriverId != driverId || !order.IsRecurring)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        order.IsRecurring = false;
        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;
        repo.Update(order);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Recurring order {OrderId} deleted", orderId);
        return Result<bool>.Success(true);
    }

    private static RecurringOrderDto MapToRecurringDto(Order order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        CustomerName = order.CustomerName,
        DeliveryAddress = order.DeliveryAddress,
        Amount = order.Amount,
        RecurrencePattern = order.RecurrencePattern ?? "daily",
        NextScheduledDate = order.ScheduledDate,
        TotalOccurrences = 0, // Would need to count child orders
        IsPaused = order.Status == OrderStatus.Postponed
    };
}

// ── Specifications ──

internal class RecurringOrdersByDriverSpec : BaseSpecification<Order>
{
    public RecurringOrdersByDriverSpec(Guid driverId)
    {
        SetCriteria(o => o.DriverId == driverId && o.IsRecurring);
        SetOrderByDescending(o => o.CreatedAt);
    }
}
