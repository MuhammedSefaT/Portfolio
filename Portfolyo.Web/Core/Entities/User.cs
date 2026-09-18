namespace Portfolyo.Web.Core.Entities;

/// <summary>
/// Yönetim paneline giriş yapan kullanıcı.
/// </summary>
public class User : BaseEntity
{
    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// PasswordHasher tarafından üretilen değer; salt hash'in içinde taşınır.
    /// Parolanın kendisi hiçbir zaman saklanmaz.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public string? ProfileImagePath { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }
}
