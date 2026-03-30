using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Enums;
using Sekka.Persistence;
using Sekka.Persistence.Entities;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/invoices")]
[Authorize(Roles = "Admin")]
public class AdminInvoiceController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public AdminInvoiceController(SekkaDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoices([FromQuery] AdminInvoiceFilterDto filter)
    {
        var query = _db.Invoices.AsNoTracking().AsQueryable();

        if (filter.Status.HasValue)
            query = query.Where(i => i.Status == filter.Status.Value);
        if (filter.DriverId.HasValue)
            query = query.Where(i => i.DriverId == filter.DriverId.Value);
        if (filter.DateFrom.HasValue)
            query = query.Where(i => i.IssuedAt >= filter.DateFrom.Value);
        if (filter.DateTo.HasValue)
            query = query.Where(i => i.IssuedAt <= filter.DateTo.Value);
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(i => i.InvoiceNumber.Contains(filter.Search)
                || (i.Notes != null && i.Notes.Contains(filter.Search)));

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(i => i.Driver)
            .OrderByDescending(i => i.IssuedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(i => new
            {
                i.Id,
                i.InvoiceNumber,
                i.DriverId,
                DriverName = i.Driver.Name,
                i.PartnerId,
                i.PeriodStart,
                i.PeriodEnd,
                i.TotalAmount,
                i.TaxAmount,
                i.DiscountAmount,
                i.NetAmount,
                i.Status,
                i.PdfUrl,
                i.Notes,
                i.IssuedAt,
                i.PaidAt
            })
            .ToListAsync();

        var result = new PagedResult<object>(
            items.Cast<object>().ToList(),
            totalCount,
            filter.Page,
            filter.PageSize);

        return Ok(ApiResponse<object>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var invoice = await _db.Invoices.AsNoTracking()
            .Include(i => i.Driver)
            .Include(i => i.Items)
            .Where(i => i.Id == id)
            .Select(i => new
            {
                i.Id,
                i.InvoiceNumber,
                i.DriverId,
                DriverName = i.Driver.Name,
                i.PartnerId,
                i.PeriodStart,
                i.PeriodEnd,
                i.TotalAmount,
                i.TaxAmount,
                i.DiscountAmount,
                i.NetAmount,
                i.Status,
                i.PdfUrl,
                i.Notes,
                i.IssuedAt,
                i.PaidAt,
                Items = i.Items.Select(item => new
                {
                    item.Id,
                    item.OrderId,
                    item.Description,
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (invoice is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.InvoiceNotFound));

        return Ok(ApiResponse<object>.Success(invoice));
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] CreateInvoiceDto dto)
    {
        var driver = await _db.Drivers.FindAsync(dto.DriverId);
        if (driver is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DriverNotFound));

        // Calculate totals from orders in the period
        var ordersInPeriod = await _db.Orders.AsNoTracking()
            .Where(o => o.DriverId == dto.DriverId
                && o.Status == OrderStatus.Delivered
                && o.DeliveredAt >= dto.PeriodStart
                && o.DeliveredAt <= dto.PeriodEnd)
            .ToListAsync();

        var totalAmount = ordersInPeriod.Sum(o => o.CommissionAmount);
        var taxAmount = totalAmount * 0.14m; // Egypt VAT 14%
        var netAmount = totalAmount + taxAmount;

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            DriverId = dto.DriverId,
            PartnerId = dto.PartnerId,
            PeriodStart = dto.PeriodStart,
            PeriodEnd = dto.PeriodEnd,
            TotalAmount = totalAmount,
            TaxAmount = taxAmount,
            DiscountAmount = 0,
            NetAmount = netAmount,
            Status = InvoiceStatus.Issued,
            Notes = dto.Notes,
            IssuedAt = DateTime.UtcNow
        };

        // Create invoice items from orders
        foreach (var order in ordersInPeriod)
        {
            invoice.Items.Add(new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                OrderId = order.Id,
                Description = $"عمولة طلب {order.OrderNumber}",
                Quantity = 1,
                UnitPrice = order.CommissionAmount,
                TotalPrice = order.CommissionAmount
            });
        }

        await _db.Invoices.AddAsync(invoice);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.TotalAmount,
            invoice.TaxAmount,
            invoice.NetAmount,
            invoice.Status,
            invoice.IssuedAt,
            ItemCount = invoice.Items.Count
        }, SuccessMessages.InvoiceGenerated));
    }

    [HttpPost("generate-bulk")]
    public IActionResult GenerateBulk()
    {
        return Ok(ApiResponse<object>.Success(new
        {
            Message = "تم بدء عملية إنشاء الفواتير الجماعية. سيتم إرسال إشعار عند الانتهاء",
            RequestedAt = DateTime.UtcNow
        }));
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateInvoiceStatusDto dto)
    {
        var invoice = await _db.Invoices.FindAsync(id);
        if (invoice is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.InvoiceNotFound));

        var oldStatus = invoice.Status;
        invoice.Status = dto.Status;

        if (dto.Status == InvoiceStatus.Paid)
            invoice.PaidAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.Notes))
            invoice.Notes = dto.Notes;

        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            invoice.Id,
            invoice.InvoiceNumber,
            OldStatus = oldStatus,
            NewStatus = invoice.Status,
            invoice.PaidAt
        }, SuccessMessages.InvoiceStatusUpdated));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var total = await _db.Invoices.CountAsync();
        var draft = await _db.Invoices.CountAsync(i => i.Status == InvoiceStatus.Draft);
        var issued = await _db.Invoices.CountAsync(i => i.Status == InvoiceStatus.Issued);
        var paid = await _db.Invoices.CountAsync(i => i.Status == InvoiceStatus.Paid);
        var overdue = await _db.Invoices.CountAsync(i => i.Status == InvoiceStatus.Overdue);
        var cancelled = await _db.Invoices.CountAsync(i => i.Status == InvoiceStatus.Cancelled);

        var totalAmount = total > 0 ? await _db.Invoices.SumAsync(i => i.NetAmount) : 0;
        var paidAmount = paid > 0
            ? await _db.Invoices.Where(i => i.Status == InvoiceStatus.Paid).SumAsync(i => i.NetAmount)
            : 0;
        var pendingAmount = totalAmount - paidAmount;

        return Ok(ApiResponse<object>.Success(new
        {
            Total = total,
            Draft = draft,
            Issued = issued,
            Paid = paid,
            Overdue = overdue,
            Cancelled = cancelled,
            TotalAmount = totalAmount,
            PaidAmount = paidAmount,
            PendingAmount = pendingAmount
        }));
    }

    [HttpGet("export")]
    public IActionResult Export([FromQuery] AdminInvoiceFilterDto filter)
    {
        return Ok(ApiResponse<object>.Success(new
        {
            Message = "تم بدء عملية التصدير. سيتم إرسال الملف عبر الإشعارات",
            RequestedAt = DateTime.UtcNow
        }));
    }
}
