using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Application.DTOs.RolePermissions;

/// <summary>
/// Bir rolün yetkilerini toplu olarak belirler.
/// Listede olmayan yetkiler rolden kaldırılır.
/// </summary>
public record RolePermissionAssignDto(
    Guid RoleId,
    IReadOnlyList<Permission> Permissions);
