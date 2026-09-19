namespace Portfolyo.Web.Application.DTOs.UserRoles;

/// <summary>
/// Bir kullanıcının rollerini toplu olarak belirler.
/// Listede olmayan roller kullanıcıdan kaldırılır.
/// </summary>
public record UserRoleAssignDto(
    Guid UserId,
    IReadOnlyList<Guid> RoleIds);
