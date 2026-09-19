using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Application.DTOs.RolePermissions;

/// <summary>Role tek bir yetki ekler.</summary>
public record RolePermissionCreateDto(
    Guid RoleId,
    Permission Permission);
