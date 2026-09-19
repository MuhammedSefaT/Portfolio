using System.Security.Claims;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.Auth;

namespace Portfolyo.Web.Application.Abstractions.Services;

/// <summary>
/// Giriş işlemi. Parolayı doğrular, kullanıcının rol ve yetkilerini claim'lere yazar.
/// Çereze yazma işini controller yapar; bu servis HttpContext bilmez.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Başarılıysa oturum açmaya hazır ClaimsPrincipal döner.
    /// Parola hatalıysa ValidationError, hesap kapalıysa Forbidden döner.
    /// </summary>
    Task<Result<ClaimsPrincipal>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
}
