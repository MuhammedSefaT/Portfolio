namespace Portfolyo.Web.Application.DTOs.UserRoles;

/// <summary>Tek bir rol ataması ekler.</summary>
public record UserRoleCreateDto(
    Guid UserId,
    Guid RoleId);
