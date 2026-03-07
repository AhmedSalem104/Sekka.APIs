namespace Sekka.Core.DTOs.Admin;
public class RoleDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int UsersCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class CreateRoleDto
{
    public string Name { get; set; } = null!;
}
public class UpdateRoleDto
{
    public string Name { get; set; } = null!;
}
public class AssignRoleDto
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = null!;
}
public class RevokeRoleDto
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = null!;
}
