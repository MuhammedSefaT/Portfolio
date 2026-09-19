namespace Portfolyo.Web.Application.DTOs.Roles;

/// <summary>
/// Rol listeleme ekranında kullanılır.
/// </summary>
public record RoleDto(
    Guid Id,
    string Name,
    string NormalizedName,
    string? Description,
    DateTime CreatedAt);
