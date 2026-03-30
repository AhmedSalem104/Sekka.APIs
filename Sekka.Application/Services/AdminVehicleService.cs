using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class AdminVehicleService : IAdminVehicleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminVehicleService> _logger;

    public AdminVehicleService(IUnitOfWork unitOfWork, ILogger<AdminVehicleService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task<Result<PagedResult<AdminVehicleDto>>> GetVehiclesAsync(AdminVehicleFilterDto filter)
    {
        var result = new PagedResult<AdminVehicleDto>(new List<AdminVehicleDto>(), 0, filter.Page, filter.PageSize);
        return Task.FromResult(Result<PagedResult<AdminVehicleDto>>.Success(result));
    }

    public Task<Result<AdminVehicleDetailDto>> GetByIdAsync(Guid id)
    {
        var dto = new AdminVehicleDetailDto
        {
            Id = id,
            DriverName = string.Empty,
            PlateNumber = string.Empty,
            MakeModel = string.Empty
        };
        return Task.FromResult(Result<AdminVehicleDetailDto>.Success(dto));
    }

    public Task<Result<bool>> ApproveAsync(Guid id)
    {
        _logger.LogInformation("Vehicle {VehicleId} approved", id);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public Task<Result<bool>> RejectAsync(Guid id, RejectVehicleDto dto)
    {
        _logger.LogInformation("Vehicle {VehicleId} rejected: {Reason}", id, dto.Reason);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public Task<Result<bool>> FlagMaintenanceAsync(Guid id, FlagMaintenanceDto dto)
    {
        _logger.LogInformation("Vehicle {VehicleId} flagged for maintenance: {Type}", id, dto.MaintenanceType);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public Task<Result<bool>> DeactivateAsync(Guid id)
    {
        _logger.LogInformation("Vehicle {VehicleId} deactivated", id);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public Task<Result<bool>> ActivateAsync(Guid id)
    {
        _logger.LogInformation("Vehicle {VehicleId} activated", id);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public Task<Result<List<AdminVehicleDto>>> GetPendingAsync()
    {
        return Task.FromResult(Result<List<AdminVehicleDto>>.Success(new List<AdminVehicleDto>()));
    }

    public Task<Result<List<AdminVehicleDto>>> GetMaintenanceDueAsync()
    {
        return Task.FromResult(Result<List<AdminVehicleDto>>.Success(new List<AdminVehicleDto>()));
    }

    public Task<Result<VehicleFleetStatsDto>> GetStatsAsync(DateOnly? dateFrom, DateOnly? dateTo)
    {
        return Task.FromResult(Result<VehicleFleetStatsDto>.Success(new VehicleFleetStatsDto()));
    }

    public Task<Result<List<VehicleTypeBreakdownDto>>> GetByTypeAsync()
    {
        return Task.FromResult(Result<List<VehicleTypeBreakdownDto>>.Success(new List<VehicleTypeBreakdownDto>()));
    }
}
