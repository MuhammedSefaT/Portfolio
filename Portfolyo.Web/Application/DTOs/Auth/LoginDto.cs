namespace Portfolyo.Web.Application.DTOs.Auth;

/// <summary>
/// Giriş formu girdisi. Kullanıcı adı veya e-posta ile giriş yapılabilir.
/// </summary>
public record LoginDto(
    string UserNameOrEmail,
    string Password,
    bool RememberMe = false);
