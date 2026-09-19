using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.Users;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Abstractions.Services;

/// <summary>
/// Kullanıcı servisi. Ortak CRUD IService'ten gelir, kullanıcıya özel işler burada.
/// </summary>
public interface IUserService : IService<User, UserDto, UserCreateDto, UserUpdateDto>
{
    Task<Result<UserDto>> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>Parola değiştirir. Mevcut parola yanlışsa doğrulama hatası döner.</summary>
    Task<Result> ChangePasswordAsync(
        Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>Kullanıcı adı/e-posta ve parolayı doğrular; başarılıysa kullanıcıyı döner.</summary>
    Task<Result<UserDto>> VerifyPasswordAsync(
        string userNameOrEmail, string password, CancellationToken cancellationToken = default);

    /// <summary>Son giriş zamanını günceller.</summary>
    Task<Result> RecordLoginAsync(Guid userId, CancellationToken cancellationToken = default);
}
