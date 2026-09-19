using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.RolePermissions;
using Portfolyo.Web.Core.Entities;
using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Application.Abstractions.Services;

/// <summary>
/// Rol - yetki atamaları. Ortak CRUD IService'ten gelir;
/// burada yalnızca bu entity'ye özel işler tanımlıdır.
/// </summary>
public interface IRolePermissionService
    : IService<RolePermission, RolePermissionDto, RolePermissionCreateDto, RolePermissionUpdateDto>
{
    Task<Result<IReadOnlyList<RolePermissionDto>>> GetByRoleAsync(
        Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolün yetkilerini listedeki son duruma göre ayarlar:
    /// eksikler eklenir, listede olmayanlar kaldırılır.
    /// </summary>
    Task<Result> AssignAsync(RolePermissionAssignDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kullanıcının tüm rollerinden gelen yetkilerin birleşimi.
    /// Girişte claim olarak yazılmak için kullanılır.
    /// </summary>
    Task<Result<IReadOnlyList<Permission>>> GetByUserAsync(
        Guid userId, CancellationToken cancellationToken = default);

    Task<Result<bool>> UserHasPermissionAsync(
        Guid userId, Permission permission, CancellationToken cancellationToken = default);
}
