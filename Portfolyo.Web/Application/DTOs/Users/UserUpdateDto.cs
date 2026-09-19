namespace Portfolyo.Web.Application.DTOs.Users;

/// <summary>
/// Kullanıcı bilgilerini güncelleme girdisi. Parola bu akışta değiştirilmez.
/// </summary>
public record UserUpdateDto(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string? ProfileImagePath,
    bool IsActive);
