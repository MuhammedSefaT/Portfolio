using Microsoft.AspNetCore.Identity;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Infrastructure.Services;

/// <summary>
/// ASP.NET Core'un PasswordHasher'ını kullanır (PBKDF2, salt hash'in içinde taşınır).
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(new User(), password);

    public bool Verify(string hash, string password)
    {
        if (string.IsNullOrWhiteSpace(hash) || string.IsNullOrEmpty(password))
        {
            return false;
        }

        var result = _hasher.VerifyHashedPassword(new User(), hash, password);

        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
