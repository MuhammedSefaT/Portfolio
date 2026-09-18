namespace Portfolyo.Web.Core.Entities;

/// <summary>
/// Kullanıcı - rol eşleşmesi. Aynı çiftin tekrar etmemesi servis katmanında doğrulanır.
/// </summary>
public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
