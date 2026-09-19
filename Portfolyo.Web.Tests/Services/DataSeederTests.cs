using Microsoft.Extensions.DependencyInjection;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.DTOs.RolePermissions;
using Portfolyo.Web.Core.Enums;
using Portfolyo.Web.Infrastructure.Persistence.Seed;

namespace Portfolyo.Web.Tests.Services;

public class DataSeederTests : ServiceTestBase
{
    private const string Parola = "SeedParola123";

    protected override void Yapilandir(IServiceCollection services)
        => services.Configure<SeedOptions>(options =>
        {
            options.Enabled = true;
            options.AdminUserName = "sefa";
            options.AdminEmail = "sefatayaz@gmail.com";
            options.AdminFirstName = "Muhammed Sefa";
            options.AdminLastName = "Tayaz";
            options.AdminRoleName = "Admin";
            options.AdminPassword = Parola;
        });

    private async Task SeedEtAsync()
    {
        using var scope = YeniIstek();
        var sonuc = await Servis<DataSeeder>(scope).SeedAsync();

        Assert.True(sonuc.IsSuccess, sonuc.Message);
    }

    [Fact]
    public async Task SeedAsync_Admin_Rolunu_Tum_Yetkilerle_Olusturur()
    {
        await SeedEtAsync();

        using var scope = YeniIstek();

        var rol = await Servis<IRoleService>(scope).GetByNameAsync("Admin");
        Assert.True(rol.IsSuccess);

        var detay = await Servis<IRoleService>(scope).GetDetailAsync(rol.Data!.Id);

        // Enum'daki her yetki (None hariç) role verilmiş olmalı.
        Assert.Equal(DataSeeder.AllPermissions.Count, detay.Data!.Permissions.Count);
        Assert.DoesNotContain(Permission.None, detay.Data.Permissions);
        Assert.Contains(Permission.SettingUpdate, detay.Data.Permissions);
    }

    [Fact]
    public async Task SeedAsync_Yonetici_Kullanicisini_Olusturur_Ve_Role_Atar()
    {
        await SeedEtAsync();

        using var scope = YeniIstek();

        var kullanici = await Servis<IUserService>(scope).GetByUserNameAsync("sefa");

        Assert.True(kullanici.IsSuccess);
        Assert.Equal("Muhammed Sefa Tayaz", kullanici.Data!.FullName);
        Assert.True(kullanici.Data.IsActive);

        // Yapılandırılan parola ile giriş yapılabilmeli.
        Assert.True((await Servis<IUserService>(scope).VerifyPasswordAsync("sefa", Parola)).IsSuccess);

        var roller = await Servis<IUserRoleService>(scope).GetByUserAsync(kullanici.Data.Id);
        var atama = Assert.Single(roller.Data!);
        Assert.Equal("Admin", atama.RoleName);

        // Kullanıcı rol üzerinden tüm yetkilere sahip olmalı.
        var yetkiler = await Servis<IRolePermissionService>(scope).GetByUserAsync(kullanici.Data.Id);
        Assert.Equal(DataSeeder.AllPermissions.Count, yetkiler.Data!.Count);
    }

    [Fact]
    public async Task SeedAsync_Tekrar_Calistirilinca_Kopya_Olusturmaz()
    {
        await SeedEtAsync();
        await SeedEtAsync();
        await SeedEtAsync();

        using var scope = YeniIstek();

        Assert.Equal(1, (await Servis<IRoleService>(scope).CountAsync()).Data);
        Assert.Equal(1, (await Servis<IUserService>(scope).CountAsync()).Data);
        Assert.Equal(1, (await Servis<IUserRoleService>(scope).CountAsync()).Data);
        Assert.Equal(DataSeeder.AllPermissions.Count, (await Servis<IRolePermissionService>(scope).CountAsync()).Data);
    }

    [Fact]
    public async Task SeedAsync_Var_Olan_Kullanicinin_Parolasini_Degistirmez()
    {
        await SeedEtAsync();

        Guid kullaniciId;

        using (var scope = YeniIstek())
        {
            kullaniciId = (await Servis<IUserService>(scope).GetByUserNameAsync("sefa")).Data!.Id;

            var degistir = await Servis<IUserService>(scope)
                .ChangePasswordAsync(kullaniciId, Parola, "BenimYeniParolam1");

            Assert.True(degistir.IsSuccess);
        }

        await SeedEtAsync();

        using (var scope = YeniIstek())
        {
            var servis = Servis<IUserService>(scope);

            // Seed, kullanıcı zaten varsa parolasına dokunmamalı.
            Assert.True((await servis.VerifyPasswordAsync("sefa", "BenimYeniParolam1")).IsSuccess);
            Assert.True((await servis.VerifyPasswordAsync("sefa", Parola)).IsFailure);
        }
    }

    [Fact]
    public async Task SeedAsync_Rolden_Silinen_Yetkiyi_Geri_Tamamlar()
    {
        await SeedEtAsync();

        Guid rolId;

        using (var scope = YeniIstek())
        {
            rolId = (await Servis<IRoleService>(scope).GetByNameAsync("Admin")).Data!.Id;

            // Yetkilerden birini elle çıkar.
            var eksikListe = DataSeeder.AllPermissions.Where(x => x != Permission.SettingUpdate).ToList();

            var sonuc = await Servis<IRolePermissionService>(scope)
                .AssignAsync(new RolePermissionAssignDto(rolId, eksikListe));

            Assert.True(sonuc.IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var detay = await Servis<IRoleService>(scope).GetDetailAsync(rolId);
            Assert.DoesNotContain(Permission.SettingUpdate, detay.Data!.Permissions);
        }

        await SeedEtAsync();

        using (var scope = YeniIstek())
        {
            var detay = await Servis<IRoleService>(scope).GetDetailAsync(rolId);

            Assert.Equal(DataSeeder.AllPermissions.Count, detay.Data!.Permissions.Count);
            Assert.Contains(Permission.SettingUpdate, detay.Data.Permissions);
        }
    }

    [Fact]
    public void AllPermissions_None_Icermez_Ve_Enum_ile_Ayni_Sayidadir()
    {
        Assert.DoesNotContain(Permission.None, DataSeeder.AllPermissions);
        Assert.Equal(Enum.GetValues<Permission>().Length - 1, DataSeeder.AllPermissions.Count);
    }
}
