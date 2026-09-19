using Portfolyo.Web.Application.Common;

namespace Portfolyo.Web.Application.DTOs.Roles;

/// <summary>
/// Rol güncelleme girdisi.
/// </summary>
public record RoleUpdateDto(
    Guid Id,
    string Name,
    string? Description) : IHasId;
