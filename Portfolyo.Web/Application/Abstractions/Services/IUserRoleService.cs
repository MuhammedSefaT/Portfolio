using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.UserRoles;

namespace Portfolyo.Web.Application.Abstractions.Services;

/// <summary>
/// Kullanıcı - rol atamaları. Bir atama "düzenlenmez", eklenir veya kaldırılır;
/// bu yüzden ortak CRUD sözleşmesi yerine kendi metotlarını taşır.
/// </summary>
public interface IUserRoleService
{
    /// <summary>Kullanıcının rollerini ad bilgileriyle birlikte döner.</summary>
    Task<Result<IReadOnlyList<UserRoleDto>>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<Guid>>> GetRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kullanıcının rollerini listedeki son duruma göre ayarlar:
    /// eksikler eklenir, listede olmayanlar kaldırılır.
    /// </summary>
    Task<Result> AssignAsync(UserRoleAssignDto dto, CancellationToken cancellationToken = default);

    Task<Result> RemoveAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}
