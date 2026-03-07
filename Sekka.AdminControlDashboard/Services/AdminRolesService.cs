using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.AdminControlDashboard.Services;

public class AdminRolesService : IAdminRolesService
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<Driver> _userManager;
    private readonly ILogger<AdminRolesService> _logger;

    public AdminRolesService(RoleManager<IdentityRole<Guid>> roleManager, UserManager<Driver> userManager, ILogger<AdminRolesService> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<List<RoleDto>>> GetRolesAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var dtos = new List<RoleDto>();
        foreach (var role in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            dtos.Add(new RoleDto
            {
                Id = role.Id.ToString(),
                Name = role.Name!,
                UsersCount = usersInRole.Count
            });
        }
        return Result<List<RoleDto>>.Success(dtos);
    }

    public async Task<Result<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
    {
        if (await _roleManager.RoleExistsAsync(dto.Name))
            return Result<RoleDto>.Conflict(ErrorMessages.RoleAlreadyExists);

        var role = new IdentityRole<Guid> { Name = dto.Name };
        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
            return Result<RoleDto>.BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Result<RoleDto>.Success(new RoleDto { Id = role.Id.ToString(), Name = role.Name! });
    }

    public async Task<Result<RoleDto>> UpdateRoleAsync(Guid id, UpdateRoleDto dto)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null) return Result<RoleDto>.NotFound(ErrorMessages.RoleNotFound);

        role.Name = dto.Name;
        await _roleManager.UpdateAsync(role);

        return Result<RoleDto>.Success(new RoleDto { Id = role.Id.ToString(), Name = role.Name! });
    }

    public async Task<Result<bool>> DeleteRoleAsync(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null) return Result<bool>.NotFound(ErrorMessages.RoleNotFound);

        await _roleManager.DeleteAsync(role);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> AssignRoleAsync(AssignRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user == null) return Result<bool>.NotFound(ErrorMessages.UserNotFound);

        var result = await _userManager.AddToRoleAsync(user, dto.RoleName);
        if (!result.Succeeded)
            return Result<bool>.BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RevokeRoleAsync(RevokeRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user == null) return Result<bool>.NotFound(ErrorMessages.UserNotFound);

        var result = await _userManager.RemoveFromRoleAsync(user, dto.RoleName);
        if (!result.Succeeded)
            return Result<bool>.BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<RoleDto>>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result<List<RoleDto>>.NotFound(ErrorMessages.UserNotFound);

        var roleNames = await _userManager.GetRolesAsync(user);
        var roles = roleNames.Select(name => new RoleDto { Name = name }).ToList();
        return Result<List<RoleDto>>.Success(roles);
    }
}
