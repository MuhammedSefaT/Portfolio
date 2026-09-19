using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Application.DTOs.RolePermissions;

/// <summary>
/// Bir role verilmiş tek bir yetkiyi gösterir.
/// </summary>
public record RolePermissionDto(
    Guid Id,
    Guid RoleId,
    Permission Permission,
    DateTime CreatedAt);
