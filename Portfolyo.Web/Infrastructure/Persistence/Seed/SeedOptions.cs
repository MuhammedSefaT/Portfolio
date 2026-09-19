namespace Portfolyo.Web.Infrastructure.Persistence.Seed;

/// <summary>
/// İlk çalıştırmada oluşturulacak yönetici hesabının ayarları ("Seed" bölümü).
/// Parola appsettings.json'a YAZILMAZ; user-secrets veya ortam değişkeni ile verilir.
/// Verilmezse rastgele üretilir ve uygulama günlüğüne bir kez yazılır.
/// </summary>
public class SeedOptions
{
    public bool Enabled { get; set; } = true;

    public string AdminUserName { get; set; } = "admin";

    public string AdminEmail { get; set; } = "admin@localhost";

    public string AdminFirstName { get; set; } = "Site";

    public string AdminLastName { get; set; } = "Yöneticisi";

    /// <summary>Boş bırakılırsa rastgele bir parola üretilir.</summary>
    public string AdminPassword { get; set; } = string.Empty;

    public string AdminRoleName { get; set; } = "Admin";
}
