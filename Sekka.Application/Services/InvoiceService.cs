using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(IUnitOfWork unitOfWork, ILogger<InvoiceService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<InvoiceDto>>> GetInvoicesAsync(Guid driverId, InvoiceFilterDto filter)
    {
        _logger.LogInformation("GetInvoices for driver {DriverId}", driverId);

        var repo = _unitOfWork.GetRepository<Invoice, Guid>();
        var spec = new DriverInvoicesSpec(driverId, filter.Status, filter.DateFrom, filter.DateTo);
        var invoices = await repo.ListAsync(spec);

        var dtos = invoices
            .OrderByDescending(i => i.IssuedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(MapToDto)
            .ToList();

        return Result<List<InvoiceDto>>.Success(dtos);
    }

    public async Task<Result<InvoiceDto>> GetByIdAsync(Guid driverId, Guid id)
    {
        _logger.LogInformation("GetInvoiceById {InvoiceId} for driver {DriverId}", id, driverId);

        var repo = _unitOfWork.GetRepository<Invoice, Guid>();
        var invoice = await repo.GetByIdAsync(id);

        if (invoice is null || invoice.DriverId != driverId)
            return Result<InvoiceDto>.NotFound("الفاتورة غير موجودة");

        var dto = MapToDto(invoice);

        // Load invoice items
        var itemRepo = _unitOfWork.GetRepository<InvoiceItem, Guid>();
        var itemSpec = new InvoiceItemsSpec(id);
        var items = await itemRepo.ListAsync(itemSpec);

        dto.Items = items.Select(item => new InvoiceItemDto
        {
            Id = item.Id,
            OrderId = item.OrderId,
            Description = item.Description,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice
        }).ToList();

        return Result<InvoiceDto>.Success(dto);
    }

    public async Task<Result<InvoiceSummaryDto>> GetSummaryAsync(Guid driverId, DateTime dateFrom, DateTime dateTo)
    {
        _logger.LogInformation("GetInvoiceSummary for driver {DriverId} from {DateFrom} to {DateTo}", driverId, dateFrom, dateTo);

        var repo = _unitOfWork.GetRepository<Invoice, Guid>();
        var spec = new DriverInvoicesSpec(driverId, null, dateFrom, dateTo);
        var invoices = await repo.ListAsync(spec);

        return Result<InvoiceSummaryDto>.Success(new InvoiceSummaryDto
        {
            TotalInvoices = invoices.Count,
            TotalAmount = invoices.Sum(i => i.NetAmount),
            TotalPaid = invoices.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.NetAmount),
            TotalPending = invoices.Where(i => i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.Overdue).Sum(i => i.NetAmount)
        });
    }

    private static InvoiceDto MapToDto(Invoice i) => new()
    {
        Id = i.Id,
        InvoiceNumber = i.InvoiceNumber,
        DriverId = i.DriverId,
        PartnerId = i.PartnerId,
        PartnerName = null,
        PeriodStart = i.PeriodStart,
        PeriodEnd = i.PeriodEnd,
        TotalAmount = i.TotalAmount,
        TaxAmount = i.TaxAmount,
        DiscountAmount = i.DiscountAmount,
        NetAmount = i.NetAmount,
        Status = i.Status,
        PdfUrl = i.PdfUrl,
        Notes = i.Notes,
        IssuedAt = i.IssuedAt,
        PaidAt = i.PaidAt,
        Items = new List<InvoiceItemDto>()
    };
}

internal class DriverInvoicesSpec : BaseSpecification<Invoice>
{
    public DriverInvoicesSpec(Guid driverId, InvoiceStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        SetCriteria(i => i.DriverId == driverId
            && (!status.HasValue || i.Status == status.Value)
            && (!dateFrom.HasValue || i.IssuedAt >= dateFrom.Value)
            && (!dateTo.HasValue || i.IssuedAt <= dateTo.Value));
        SetOrderByDescending(i => i.IssuedAt);
    }
}

internal class InvoiceItemsSpec : BaseSpecification<InvoiceItem>
{
    public InvoiceItemsSpec(Guid invoiceId)
    {
        SetCriteria(i => i.InvoiceId == invoiceId);
        SetOrderBy(i => i.CreatedAt);
    }
}
