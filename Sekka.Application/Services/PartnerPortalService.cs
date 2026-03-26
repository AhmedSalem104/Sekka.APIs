using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Order;
using Sekka.Core.DTOs.Partner;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class PartnerPortalService : IPartnerPortalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PartnerPortalService> _logger;

    public PartnerPortalService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PartnerPortalService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<Result<PartnerDashboardDto>> GetDashboardAsync(Guid partnerId)
    {
        return Task.FromResult(Result<PartnerDashboardDto>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("لوحة تحكم الشريك")));
    }

    public Task<Result<PagedResult<OrderListDto>>> GetOrdersAsync(Guid partnerId, PartnerOrdersFilterDto filter)
    {
        return Task.FromResult(Result<PagedResult<OrderListDto>>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("طلبات الشريك")));
    }

    public Task<Result<PagedResult<SettlementDto>>> GetSettlementsAsync(Guid partnerId, SettlementFilterDto filter)
    {
        return Task.FromResult(Result<PagedResult<SettlementDto>>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("تسويات الشريك")));
    }

    public Task<Result<PartnerSettingsDto>> UpdateSettingsAsync(Guid partnerId, PartnerSettingsDto dto)
    {
        return Task.FromResult(Result<PartnerSettingsDto>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("إعدادات الشريك")));
    }

    public Task<Result<PartnerStatsDto>> GetStatsAsync(Guid partnerId, DateOnly? dateFrom, DateOnly? dateTo)
    {
        return Task.FromResult(Result<PartnerStatsDto>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("إحصائيات الشريك")));
    }

    public Task<Result<PagedResult<InvoiceDto>>> GetInvoicesAsync(Guid partnerId, InvoiceFilterDto filter)
    {
        return Task.FromResult(Result<PagedResult<InvoiceDto>>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("فواتير الشريك")));
    }
}
