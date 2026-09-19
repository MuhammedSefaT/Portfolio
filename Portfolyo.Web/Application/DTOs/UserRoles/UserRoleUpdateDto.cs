using Portfolyo.Web.Application.Common;

namespace Portfolyo.Web.Application.DTOs.UserRoles;

/// <summary>Mevcut bir atamayı başka kullanıcı/rol çiftine taşır.</summary>
public record UserRoleUpdateDto(
    Guid Id,
    Guid UserId,
    Guid RoleId) : IHasId;
