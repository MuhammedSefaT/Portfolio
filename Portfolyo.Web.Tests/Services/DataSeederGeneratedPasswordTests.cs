using Microsoft.Extensions.DependencyInjection;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Infrastructure.Persistence.Seed;

namespace Portfolyo.Web.Tests.Services;

/// <summary>
/// Parola yapılandırmada verilmediğinde rastgele üretilmeli.
/// </summary>
public class DataSeederGeneratedPasswordTests : ServiceTestBase
{
    protected override void Yapilandir(IServiceCollection services)
        => services.Configure<SeedOptions>(options =>
        {
            options.Enabled = true;
            options.AdminUserName = "admin";
            options.AdminEmail = "admin@localhost";
            options.AdminPassword = string.Empty; // Yapılandırılmadı: üretilmeli.
        });

    [Fact]
    public async Task Parola_Verilmezse_Uretilir_Ve_Hashlenmis_Olarak_Saklanir()
    {
        using (var scope = YeniIstek())
        {
            Assert.True((await Servis<DataSeeder>(scope).SeedAsync()).IsSuccess);
        }

        using var yeniScope = YeniIstek();

        var kullanici = await Servis<IUserService>(yeniScope).GetByUserNameAsync("admin");
        Assert.True(kullanici.IsSuccess);

        var icerik = await DosyaOkuAsync("users.json");

        // Üretilen parola dosyada düz metin olarak bulunmamalı, yalnızca hash olmalı.
        Assert.Contains("passwordHash", icerik);
        Assert.DoesNotContain("\"password\"", icerik);
    }
}
