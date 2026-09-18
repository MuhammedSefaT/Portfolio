namespace Portfolyo.Web.Core.Entities;

/// <summary>
/// Kullanıcılara atanan rol. Yetkiler role üzerinden verilir.
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Karşılaştırma ve arama için normalleştirilmiş ad (ör. "admin").
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    public string? Description { get; set; }
}
