using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

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

    public Task<Result<List<InvoiceDto>>> GetInvoicesAsync(Guid driverId, InvoiceFilterDto filter)
        => Task.FromResult(Result<List<InvoiceDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الفواتير")));

    public Task<Result<InvoiceDto>> GetByIdAsync(Guid driverId, Guid id)
        => Task.FromResult(Result<InvoiceDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الفواتير")));

    public Task<Result<InvoiceSummaryDto>> GetSummaryAsync(Guid driverId, DateTime dateFrom, DateTime dateTo)
        => Task.FromResult(Result<InvoiceSummaryDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الفواتير")));
}
