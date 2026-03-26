using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;

namespace Sekka.Core.Interfaces.Services;

public interface IAnalyticsService
{
    Task<Result<List<SourceBreakdownDto>>> GetSourceBreakdownAsync(Guid driverId, DateTime dateFrom, DateTime dateTo);
    Task<Result<List<CustomerProfitabilityDto>>> GetCustomerProfitabilityAsync(Guid driverId, DateTime dateFrom, DateTime dateTo);
    Task<Result<List<RegionAnalysisDto>>> GetRegionAnalysisAsync(Guid driverId, DateTime dateFrom, DateTime dateTo);
    Task<Result<List<TimeAnalysisDto>>> GetTimeAnalysisAsync(Guid driverId, DateTime dateFrom, DateTime dateTo);
    Task<Result<List<CancellationReportDto>>> GetCancellationReportAsync(Guid driverId, DateTime dateFrom, DateTime dateTo);
    Task<Result<List<ProfitabilityTrendDto>>> GetProfitabilityTrendsAsync(Guid driverId, string period);
}
