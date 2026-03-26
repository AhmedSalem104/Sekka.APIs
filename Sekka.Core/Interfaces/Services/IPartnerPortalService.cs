using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Order;
using Sekka.Core.DTOs.Partner;

namespace Sekka.Core.Interfaces.Services;

public interface IPartnerPortalService
{
    Task<Result<PartnerDashboardDto>> GetDashboardAsync(Guid partnerId);
    Task<Result<PagedResult<OrderListDto>>> GetOrdersAsync(Guid partnerId, PartnerOrdersFilterDto filter);
    Task<Result<PagedResult<SettlementDto>>> GetSettlementsAsync(Guid partnerId, SettlementFilterDto filter);
    Task<Result<PartnerSettingsDto>> UpdateSettingsAsync(Guid partnerId, PartnerSettingsDto dto);
    Task<Result<PartnerStatsDto>> GetStatsAsync(Guid partnerId, DateOnly? dateFrom, DateOnly? dateTo);
    Task<Result<PagedResult<InvoiceDto>>> GetInvoicesAsync(Guid partnerId, InvoiceFilterDto filter);
}
