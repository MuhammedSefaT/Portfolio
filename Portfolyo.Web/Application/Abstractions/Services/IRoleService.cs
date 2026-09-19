using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.Roles;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Abstractions.Services;

/// <summary>
/// Rol servisi. Rol silindiğinde ilgili yetki ve kullanıcı atamaları da temizlenir.
/// </summary>
public interface IRoleService : IService<Role, RoleDto, RoleCreateDto, RoleUpdateDto>
{
    /// <summary>Rolü yetkileriyle birlikte döner.</summary>
    Task<Result<RoleDetailDto>> GetDetailAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<RoleDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
