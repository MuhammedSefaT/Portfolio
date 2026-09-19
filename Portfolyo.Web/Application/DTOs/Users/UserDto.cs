namespace Portfolyo.Web.Application.DTOs.Users;

/// <summary>
/// Kullanıcı listeleme ve detay ekranlarında kullanılır. Parola bilgisi taşımaz.
/// </summary>
public record UserDto(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string? ProfileImagePath,
    bool IsActive,
    DateTime? LastLoginAt,
    DateTime CreatedAt)
{
    public string FullName => $"{FirstName} {LastName}".Trim();
}
