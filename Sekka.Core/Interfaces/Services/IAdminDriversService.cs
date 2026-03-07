using Sekka.Core.Common;
using Sekka.Core.DTOs.Admin;

namespace Sekka.Core.Interfaces.Services;

public interface IAdminDriversService
{
    Task<Result<PagedResult<AdminDriverDto>>> GetDriversAsync(AdminDriverFilterDto filter);
    Task<Result<AdminDriverDetailDto>> GetDriverByIdAsync(Guid id);
    Task<Result<bool>> ActivateDriverAsync(Guid id);
    Task<Result<bool>> DeactivateDriverAsync(Guid id);
    Task<Result<DriverPerformanceDto>> GetPerformanceAsync(Guid id, DateTime? fromDate, DateTime? toDate);
    Task<Result<List<DriverLocationDto>>> GetDriverLocationsAsync();
}
