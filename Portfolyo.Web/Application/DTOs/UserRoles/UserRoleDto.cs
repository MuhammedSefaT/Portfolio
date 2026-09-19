namespace Portfolyo.Web.Application.DTOs.UserRoles;

/// <summary>
/// Kullanıcı - rol ataması. Ad alanları için UserRoleDetailDto kullanılır.
/// </summary>
public record UserRoleDto(
    Guid Id,
    Guid UserId,
    Guid RoleId,
    DateTime AssignedAt);
