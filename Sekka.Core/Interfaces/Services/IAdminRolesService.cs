using Sekka.Core.Common;
using Sekka.Core.DTOs.Admin;

namespace Sekka.Core.Interfaces.Services;

public interface IAdminRolesService
{
    Task<Result<List<RoleDto>>> GetRolesAsync();
    Task<Result<RoleDto>> CreateRoleAsync(CreateRoleDto dto);
    Task<Result<RoleDto>> UpdateRoleAsync(Guid id, UpdateRoleDto dto);
    Task<Result<bool>> DeleteRoleAsync(Guid id);
    Task<Result<bool>> AssignRoleAsync(AssignRoleDto dto);
    Task<Result<bool>> RevokeRoleAsync(RevokeRoleDto dto);
    Task<Result<List<RoleDto>>> GetUserRolesAsync(Guid userId);
}
