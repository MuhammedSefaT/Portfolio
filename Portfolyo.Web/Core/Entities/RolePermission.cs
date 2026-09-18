using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Core.Entities;

/// <summary>
/// Bir role verilen tek bir yetki. Aynı rol için aynı yetkinin tekrar etmemesi
/// servis katmanında doğrulanır.
/// </summary>
public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }

    public Permission Permission { get; set; }
}
