using Sekka.Core.Common;
using Sekka.Core.DTOs.Admin;

namespace Sekka.Core.Interfaces.Services;

public interface IAdminVehicleService
{
    Task<Result<PagedResult<AdminVehicleDto>>> GetVehiclesAsync(AdminVehicleFilterDto filter);
    Task<Result<AdminVehicleDetailDto>> GetByIdAsync(Guid id);
    Task<Result<bool>> ApproveAsync(Guid id);
    Task<Result<bool>> RejectAsync(Guid id, RejectVehicleDto dto);
    Task<Result<bool>> FlagMaintenanceAsync(Guid id, FlagMaintenanceDto dto);
    Task<Result<bool>> DeactivateAsync(Guid id);
    Task<Result<bool>> ActivateAsync(Guid id);
    Task<Result<List<AdminVehicleDto>>> GetPendingAsync();
    Task<Result<List<AdminVehicleDto>>> GetMaintenanceDueAsync();
    Task<Result<VehicleFleetStatsDto>> GetStatsAsync(DateOnly? dateFrom, DateOnly? dateTo);
    Task<Result<List<VehicleTypeBreakdownDto>>> GetByTypeAsync();
}
