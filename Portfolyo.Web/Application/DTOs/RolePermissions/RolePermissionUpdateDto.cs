using Portfolyo.Web.Application.Common;
using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Application.DTOs.RolePermissions;

/// <summary>Mevcut bir yetki kaydını değiştirir.</summary>
public record RolePermissionUpdateDto(
    Guid Id,
    Guid RoleId,
    Permission Permission) : IHasId;
