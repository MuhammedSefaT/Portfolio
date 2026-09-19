namespace Portfolyo.Web.Application.Common;

/// <summary>
/// Güncelleme DTO'larının taşıdığı kimlik alanı.
/// Generic servisin hangi kaydın güncellendiğini bilmesi için gerekli.
/// </summary>
public interface IHasId
{
    Guid Id { get; }
}
