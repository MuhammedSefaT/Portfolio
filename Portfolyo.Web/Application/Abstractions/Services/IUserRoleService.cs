using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.UserRoles;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Abstractions.Services;

/// <summary>
/// Kullanıcı - rol atamaları. Ortak CRUD IService'ten gelir;
/// burada yalnızca bu entity'ye özel işler tanımlıdır.
/// </summary>
public interface IUserRoleService : IService<UserRole, UserRoleDto, UserRoleCreateDto, UserRoleUpdateDto>
{
    /// <summary>Kullanıcının rollerini ad bilgileriyle birlikte döner.</summary>
    Task<Result<IReadOnlyList<UserRoleDetailDto>>> GetByUserAsync(
        Guid userId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<Guid>>> GetRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kullanıcının rollerini listedeki son duruma göre ayarlar:
    /// eksikler eklenir, listede olmayanlar kaldırılır.
    /// </summary>
    Task<Result> AssignAsync(UserRoleAssignDto dto, CancellationToken cancellationToken = default);

    Task<Result> RemoveAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}
