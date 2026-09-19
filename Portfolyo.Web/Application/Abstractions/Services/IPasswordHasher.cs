namespace Portfolyo.Web.Application.Abstractions.Services;

/// <summary>
/// Parola hash'leme. Uygulama katmanı hangi algoritmanın kullanıldığını bilmez.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string hash, string password);
}
