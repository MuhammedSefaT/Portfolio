namespace Portfolyo.Web.Core.Enums;

/// <summary>
/// Rollere atanan yetkiler. Sayısal değerler JSON dosyalarında saklandığı için
/// mevcut bir değer ASLA değiştirilmez; yeni yetki yalnızca sona eklenir.
/// Her modül 100'lük bir blok kullanır.
/// </summary>
public enum Permission
{
    None = 0,

    // Panel erişimi
    DashboardView = 100,

    // Kullanıcı yönetimi
    UserView = 200,
    UserCreate = 201,
    UserUpdate = 202,
    UserDelete = 203,

    // Rol ve yetki yönetimi
    RoleView = 300,
    RoleCreate = 301,
    RoleUpdate = 302,
    RoleDelete = 303,
    RoleAssignPermission = 304,

    // İçerik yönetimi (proje, deneyim, yetenek vb.)
    ContentView = 400,
    ContentCreate = 401,
    ContentUpdate = 402,
    ContentDelete = 403,
    ContentPublish = 404,

    // İletişim formu mesajları
    MessageView = 500,
    MessageDelete = 501,

    // Site ayarları
    SettingView = 600,
    SettingUpdate = 601
}
