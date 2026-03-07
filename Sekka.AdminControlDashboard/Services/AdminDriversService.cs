using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.AdminControlDashboard.Services;

public class AdminDriversService : IAdminDriversService
{
    private readonly UserManager<Driver> _userManager;
    private readonly ILogger<AdminDriversService> _logger;

    public AdminDriversService(UserManager<Driver> userManager, ILogger<AdminDriversService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<PagedResult<AdminDriverDto>>> GetDriversAsync(AdminDriverFilterDto filter)
    {
        var query = _userManager.Users.AsQueryable();

        if (filter.IsActive.HasValue)
            query = query.Where(d => d.IsActive == filter.IsActive.Value);
        if (filter.IsOnline.HasValue)
            query = query.Where(d => d.IsOnline == filter.IsOnline.Value);
        if (!string.IsNullOrEmpty(filter.SearchTerm))
            query = query.Where(d => d.Name.Contains(filter.SearchTerm) || d.PhoneNumber!.Contains(filter.SearchTerm));

        var total = await query.CountAsync();
        var drivers = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(d => new AdminDriverDto
            {
                Id = d.Id,
                Name = d.Name,
                Phone = d.PhoneNumber ?? "",
                IsActive = d.IsActive,
                IsOnline = d.IsOnline,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync();

        return Result<PagedResult<AdminDriverDto>>.Success(
            new PagedResult<AdminDriverDto>(drivers, total, filter.Page, filter.PageSize));
    }

    public async Task<Result<AdminDriverDetailDto>> GetDriverByIdAsync(Guid id)
    {
        var driver = await _userManager.FindByIdAsync(id.ToString());
        if (driver == null)
            return Result<AdminDriverDetailDto>.NotFound(ErrorMessages.DriverNotFound);

        return Result<AdminDriverDetailDto>.Success(new AdminDriverDetailDto
        {
            Id = driver.Id,
            Name = driver.Name,
            Phone = driver.PhoneNumber ?? "",
            Email = driver.Email,
            ProfileImageUrl = driver.ProfileImageUrl,
            IsActive = driver.IsActive,
            IsOnline = driver.IsOnline,
            CashOnHand = driver.CashOnHand,
            LastLocationUpdate = driver.LastLocationUpdate,
            CreatedAt = driver.CreatedAt
        });
    }

    public async Task<Result<bool>> ActivateDriverAsync(Guid id)
    {
        var driver = await _userManager.FindByIdAsync(id.ToString());
        if (driver == null) return Result<bool>.NotFound(ErrorMessages.DriverNotFound);

        driver.IsActive = true;
        driver.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(driver);

        _logger.LogInformation("Driver {DriverId} activated", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeactivateDriverAsync(Guid id)
    {
        var driver = await _userManager.FindByIdAsync(id.ToString());
        if (driver == null) return Result<bool>.NotFound(ErrorMessages.DriverNotFound);

        driver.IsActive = false;
        driver.IsOnline = false;
        driver.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(driver);

        _logger.LogInformation("Driver {DriverId} deactivated", id);
        return Result<bool>.Success(true);
    }

    public Task<Result<DriverPerformanceDto>> GetPerformanceAsync(Guid id, DateTime? fromDate, DateTime? toDate)
    {
        // TODO: Calculate from Orders table
        return Task.FromResult(Result<DriverPerformanceDto>.Success(new DriverPerformanceDto()));
    }

    public async Task<Result<List<DriverLocationDto>>> GetDriverLocationsAsync()
    {
        var onlineDrivers = await _userManager.Users
            .Where(d => d.IsOnline && d.LastKnownLatitude.HasValue)
            .Select(d => new DriverLocationDto
            {
                DriverId = d.Id,
                DriverName = d.Name,
                Latitude = d.LastKnownLatitude,
                Longitude = d.LastKnownLongitude,
                IsOnline = true,
                LastUpdate = d.LastLocationUpdate
            })
            .ToListAsync();

        return Result<List<DriverLocationDto>>.Success(onlineDrivers);
    }
}
