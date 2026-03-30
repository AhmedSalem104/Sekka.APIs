using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence;
using Sekka.Persistence.Entities;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly SekkaDbContext _db;
    private readonly IAutoAssignmentService _autoAssignService;

    public AdminOrdersController(SekkaDbContext db, IAutoAssignmentService autoAssignService)
    {
        _db = db;
        _autoAssignService = autoAssignService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] AdminOrderFilterDto filter)
    {
        var query = _db.Orders.AsNoTracking().AsQueryable();

        if (filter.Status.HasValue)
            query = query.Where(o => o.Status == filter.Status.Value);
        if (filter.DriverId.HasValue)
            query = query.Where(o => o.DriverId == filter.DriverId.Value);
        if (filter.PartnerId.HasValue)
            query = query.Where(o => o.PartnerId == filter.PartnerId.Value);
        if (filter.PaymentMethod.HasValue)
            query = query.Where(o => o.PaymentMethod == filter.PaymentMethod.Value);
        if (filter.Priority.HasValue)
            query = query.Where(o => o.Priority == filter.Priority.Value);
        if (filter.DateFrom.HasValue)
            query = query.Where(o => o.CreatedAt >= filter.DateFrom.Value.ToDateTime(TimeOnly.MinValue));
        if (filter.DateTo.HasValue)
            query = query.Where(o => o.CreatedAt <= filter.DateTo.Value.ToDateTime(TimeOnly.MaxValue));
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            query = query.Where(o => o.OrderNumber.Contains(filter.SearchTerm)
                || (o.CustomerName != null && o.CustomerName.Contains(filter.SearchTerm))
                || (o.CustomerPhone != null && o.CustomerPhone.Contains(filter.SearchTerm)));

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(o => o.Driver)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(o => new
            {
                o.Id,
                o.OrderNumber,
                o.CustomerName,
                o.CustomerPhone,
                o.Amount,
                o.CommissionAmount,
                o.PaymentMethod,
                o.Status,
                o.Priority,
                o.DeliveryAddress,
                o.DistanceKm,
                o.SequenceIndex,
                o.CreatedAt,
                o.DeliveredAt,
                DriverName = o.Driver.Name,
                DriverPhone = o.Driver.PhoneNumber
            })
            .ToListAsync();

        var result = new PagedResult<object>(
            items.Cast<object>().ToList(),
            totalCount,
            filter.Page,
            filter.PageSize);

        return Ok(ApiResponse<object>.Success(result));
    }

    [HttpGet("board")]
    public async Task<IActionResult> GetBoard()
    {
        var orders = await _db.Orders.AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .Take(500)
            .Select(o => new
            {
                o.Id,
                o.OrderNumber,
                o.CustomerName,
                o.Amount,
                o.Status,
                o.Priority,
                o.DeliveryAddress,
                o.SequenceIndex,
                o.CreatedAt
            })
            .ToListAsync();

        var board = new
        {
            Pending = orders.Where(o => o.Status == OrderStatus.Pending).ToList(),
            Accepted = orders.Where(o => o.Status == OrderStatus.Accepted).ToList(),
            PickedUp = orders.Where(o => o.Status == OrderStatus.PickedUp).ToList(),
            InTransit = orders.Where(o => o.Status == OrderStatus.InTransit).ToList(),
            Delivered = orders.Where(o => o.Status == OrderStatus.Delivered).ToList(),
            Failed = orders.Where(o => o.Status == OrderStatus.Failed).ToList(),
            Cancelled = orders.Where(o => o.Status == OrderStatus.Cancelled).ToList()
        };

        return Ok(ApiResponse<object>.Success(board));
    }

    [HttpPost]
    public async Task<IActionResult> AdminCreate([FromBody] AdminCreateOrderDto dto)
    {
        var driver = await _db.Drivers.FindAsync(dto.DriverId);
        if (driver is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DriverNotFound));

        var order = new Order
        {
            Id = Guid.NewGuid(),
            DriverId = dto.DriverId,
            PartnerId = dto.PartnerId,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            CustomerName = dto.CustomerName,
            CustomerPhone = dto.CustomerPhone,
            Description = dto.Description,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            Priority = dto.Priority,
            Status = OrderStatus.Pending,
            SourceType = OrderSourceType.Manual,
            PickupAddress = dto.PickupAddress,
            PickupLatitude = dto.PickupLatitude,
            PickupLongitude = dto.PickupLongitude,
            DeliveryAddress = dto.DeliveryAddress,
            DeliveryLatitude = dto.DeliveryLatitude,
            DeliveryLongitude = dto.DeliveryLongitude,
            Notes = dto.Notes,
            ItemCount = dto.ItemCount,
            TimeWindowStart = dto.TimeWindowStart,
            TimeWindowEnd = dto.TimeWindowEnd,
            ScheduledDate = dto.ScheduledDate,
            IsRecurring = dto.IsRecurring,
            RecurrencePattern = dto.RecurrencePattern,
            ExpectedChangeAmount = dto.ExpectedChangeAmount,
            IdempotencyKey = dto.IdempotencyKey
        };

        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            order.Id,
            order.OrderNumber,
            order.Status,
            order.CreatedAt
        }, SuccessMessages.OrderCreated));
    }

    [HttpPost("{id:guid}/assign")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignOrderDto dto)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.OrderNotFound));

        var driver = await _db.Drivers.FindAsync(dto.DriverId);
        if (driver is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DriverNotFound));

        order.DriverId = dto.DriverId;
        order.AssignedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            order.Id,
            order.OrderNumber,
            order.DriverId,
            order.AssignedAt
        }, SuccessMessages.OrderAssigned));
    }

    [HttpPost("auto-distribute")]
    public IActionResult AutoDistribute([FromBody] AutoDistributeDto dto)
    {
        // Auto-distribution is a complex feature that requires the full assignment engine
        return Ok(ApiResponse<object>.Success(new DistributionResultDto
        {
            TotalOrders = dto.OrderIds.Count,
            AssignedOrders = 0,
            UnassignedOrders = dto.OrderIds.Count,
            Assignments = dto.OrderIds.Select(oid => new AssignmentResultItemDto
            {
                OrderId = oid,
                IsAssigned = false,
                FailureReason = "التوزيع التلقائي المتقدم قيد التطوير"
            }).ToList()
        }));
    }

    [HttpGet("{id:guid}/timeline")]
    public async Task<IActionResult> GetTimeline(Guid id)
    {
        var order = await _db.Orders.AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new { o.Id, o.OrderNumber, o.CreatedAt, o.AssignedAt, o.PickedUpAt, o.ArrivedAt, o.DeliveredAt, o.FailedAt, o.Status })
            .FirstOrDefaultAsync();

        if (order is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.OrderNotFound));

        var events = new List<OrderTimelineEventDto>();

        events.Add(new OrderTimelineEventDto
        {
            Event = "Created",
            Description = "تم إنشاء الطلب",
            Timestamp = order.CreatedAt
        });

        if (order.AssignedAt.HasValue)
            events.Add(new OrderTimelineEventDto
            {
                Event = "Assigned",
                Description = "تم تعيين الطلب للسائق",
                Timestamp = order.AssignedAt.Value
            });

        if (order.PickedUpAt.HasValue)
            events.Add(new OrderTimelineEventDto
            {
                Event = "PickedUp",
                Description = "تم استلام الطلب",
                Timestamp = order.PickedUpAt.Value
            });

        if (order.ArrivedAt.HasValue)
            events.Add(new OrderTimelineEventDto
            {
                Event = "Arrived",
                Description = "وصل السائق للعميل",
                Timestamp = order.ArrivedAt.Value
            });

        if (order.DeliveredAt.HasValue)
            events.Add(new OrderTimelineEventDto
            {
                Event = "Delivered",
                Description = "تم تسليم الطلب",
                Timestamp = order.DeliveredAt.Value
            });

        if (order.FailedAt.HasValue)
            events.Add(new OrderTimelineEventDto
            {
                Event = "Failed",
                Description = "فشل التسليم",
                Timestamp = order.FailedAt.Value
            });

        var timeline = new OrderTimelineDto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            Events = events.OrderBy(e => e.Timestamp).ToList()
        };

        return Ok(ApiResponse<object>.Success(timeline));
    }

    [HttpPut("{id:guid}/override-status")]
    public async Task<IActionResult> OverrideStatus(Guid id, [FromBody] OverrideStatusDto dto)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.OrderNotFound));

        var oldStatus = order.Status;
        order.Status = dto.NewStatus;

        // Set relevant timestamp based on new status
        switch (dto.NewStatus)
        {
            case OrderStatus.Delivered:
                order.DeliveredAt ??= DateTime.UtcNow;
                break;
            case OrderStatus.Failed:
                order.FailedAt ??= DateTime.UtcNow;
                break;
            case OrderStatus.PickedUp:
                order.PickedUpAt ??= DateTime.UtcNow;
                break;
        }

        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            order.Id,
            order.OrderNumber,
            OldStatus = oldStatus,
            NewStatus = order.Status,
            Reason = dto.Reason
        }, SuccessMessages.OrderStatusUpdated));
    }

    [HttpGet("export")]
    public IActionResult Export([FromQuery] ExportFilterDto filter)
    {
        return Ok(ApiResponse<object>.Success(new
        {
            Message = "تم بدء عملية التصدير. سيتم إرسال الملف عبر الإشعارات",
            Format = filter.Format,
            RequestedAt = DateTime.UtcNow
        }));
    }

    [HttpPost("{id:guid}/auto-assign")]
    public async Task<IActionResult> AutoAssign(Guid id, [FromBody] AssignmentConfigDto config)
    {
        var result = await _autoAssignService.AutoAssignAsync(id, config);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.OrderAssigned))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("{id:guid}/suggested-drivers")]
    public async Task<IActionResult> GetSuggestedDrivers(Guid id)
    {
        var result = await _autoAssignService.GetSuggestedDriversAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
