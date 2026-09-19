namespace Portfolyo.Web.Application.DTOs.Users;

/// <summary>
/// Yeni kullanıcı oluşturma girdisi. Parola hash'lenmeden önce servis katmanına buradan geçer.
/// </summary>
public record UserCreateDto(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string Password);
