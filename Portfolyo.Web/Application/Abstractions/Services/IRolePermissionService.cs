using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.RolePermissions;
using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Application.Abstractions.Services;

/// <summary>
/// Rol - yetki atamaları. Atama düzenlenmez, toplu olarak belirlenir;
/// bu yüzden ortak CRUD sözleşmesi yerine kendi metotlarını taşır.
/// </summary>
public interface IRolePermissionService
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
