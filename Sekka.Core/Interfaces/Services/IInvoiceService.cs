using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;

namespace Sekka.Core.Interfaces.Services;

public interface IInvoiceService
{
    Task<Result<List<InvoiceDto>>> GetInvoicesAsync(Guid driverId, InvoiceFilterDto filter);
    Task<Result<InvoiceDto>> GetByIdAsync(Guid driverId, Guid id);
    Task<Result<InvoiceSummaryDto>> GetSummaryAsync(Guid driverId, DateTime dateFrom, DateTime dateTo);
}
