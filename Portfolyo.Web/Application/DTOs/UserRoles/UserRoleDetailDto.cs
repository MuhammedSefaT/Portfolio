namespace Portfolyo.Web.Application.DTOs.UserRoles;

/// <summary>
/// Atamanın gösterim hâli. Entity'lerde navigation property olmadığı için
/// ad alanları servis katmanında users.json ve roles.json ile birleştirilir.
/// </summary>
public record UserRoleDetailDto(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid RoleId,
    string RoleName,
    DateTime AssignedAt);
