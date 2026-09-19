using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Application.DTOs.Roles;

/// <summary>
/// Rol detayı; yetkiler rolePermissions üzerinden birleştirilerek doldurulur.
/// </summary>
public record RoleDetailDto(
    Guid Id,
    string Name,
    string NormalizedName,
    string? Description,
    DateTime CreatedAt,
    IReadOnlyList<Permission> Permissions);
