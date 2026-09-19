namespace Portfolyo.Web.Application.DTOs.Roles;

/// <summary>
/// Yeni rol oluşturma girdisi. NormalizedName servis katmanında Name'den üretilir.
/// </summary>
public record RoleCreateDto(
    string Name,
    string? Description);
