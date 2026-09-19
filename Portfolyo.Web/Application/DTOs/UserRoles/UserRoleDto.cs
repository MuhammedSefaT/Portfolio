namespace Portfolyo.Web.Application.DTOs.UserRoles;

/// <summary>
/// Kullanıcının sahip olduğu rolü göstermek için kullanılır.
/// Ad alanları users.json ve roles.json ile birleştirilerek doldurulur.
/// </summary>
public record UserRoleDto(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid RoleId,
    string RoleName,
    DateTime AssignedAt);
