using System.Security.Claims;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Constants;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.Auth;
using Portfolyo.Web.Application.DTOs.RolePermissions;
using Portfolyo.Web.Application.DTOs.Roles;
using Portfolyo.Web.Application.DTOs.UserRoles;
using Portfolyo.Web.Application.DTOs.Users;
using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Tests.Services;

public class AuthServiceTests : ServiceTestBase
{
    private const string Parola = "Parola1234";

    /// <summary>Kullanıcı + Admin rolü + iki yetki hazırlar.</summary>
    private async Task<Guid> KullaniciHazirlaAsync(bool rolAta = true)
    {
        using var scope = YeniIstek();

        var kullanici = await Servis<IUserService>(scope).CreateAsync(
            new UserCreateDto("sefa", "sefatayaz@gmail.com", "Muhammed Sefa", "Tayaz", Parola));

        if (!rolAta)
        {
            return kullanici.Data!.Id;
        }

        var rol = await Servis<IRoleService>(scope).CreateAsync(new RoleCreateDto("Admin", null));

        await Servis<IRolePermissionService>(scope).AssignAsync(
            new RolePermissionAssignDto(rol.Data!.Id, new[] { Permission.DashboardView, Permission.UserView }));

        await Servis<IUserRoleService>(scope).AssignAsync(
            new UserRoleAssignDto(kullanici.Data!.Id, new[] { rol.Data.Id }));

        return kullanici.Data.Id;
    }

    [Fact]
    public async Task LoginAsync_Basarili_Girisde_Claim_Uretir()
    {
        var kullaniciId = await KullaniciHazirlaAsync();

        using var scope = YeniIstek();

        var sonuc = await Servis<IAuthService>(scope).LoginAsync(new LoginDto("sefa", Parola));

        Assert.True(sonuc.IsSuccess);

        var principal = sonuc.Data!;
        Assert.True(principal.Identity?.IsAuthenticated);

        Assert.Equal(kullaniciId.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal("sefa", principal.FindFirst(ClaimTypes.Name)?.Value);
        Assert.Equal("sefatayaz@gmail.com", principal.FindFirst(ClaimTypes.Email)?.Value);
        Assert.True(principal.IsInRole("Admin"));

        // Yetkiler claim olarak yazılmalı.
        var yetkiClaimleri = principal.FindAll(AuthConstants.PermissionClaimType).Select(x => x.Value).ToList();

        Assert.Equal(2, yetkiClaimleri.Count);
        Assert.Contains(AuthConstants.ClaimValue(Permission.DashboardView), yetkiClaimleri);
        Assert.Contains(AuthConstants.ClaimValue(Permission.UserView), yetkiClaimleri);
        Assert.DoesNotContain(AuthConstants.ClaimValue(Permission.SettingUpdate), yetkiClaimleri);
    }

    [Fact]
    public async Task LoginAsync_Eposta_ile_de_Giris_Yapilabilir()
    {
        await KullaniciHazirlaAsync();

        using var scope = YeniIstek();

        var sonuc = await Servis<IAuthService>(scope)
            .LoginAsync(new LoginDto("SEFATAYAZ@gmail.com", Parola));

        Assert.True(sonuc.IsSuccess);
    }

    [Fact]
    public async Task LoginAsync_Yanlis_Parolada_ValidationError_Dondurur()
    {
        await KullaniciHazirlaAsync();

        using var scope = YeniIstek();

        var sonuc = await Servis<IAuthService>(scope).LoginAsync(new LoginDto("sefa", "YanlisParola1"));

        Assert.True(sonuc.IsFailure);
        Assert.Equal(ResultStatus.ValidationError, sonuc.Status);
        Assert.Null(sonuc.Data);
        Assert.Equal("Kullanıcı adı veya parola hatalı.", sonuc.Errors[0].Message);
    }

    [Fact]
    public async Task LoginAsync_Olmayan_Kullanicida_Ayni_Mesaji_Dondurur()
    {
        await KullaniciHazirlaAsync();

        using var scope = YeniIstek();
        var servis = Servis<IAuthService>(scope);

        var olmayan = await servis.LoginAsync(new LoginDto("yokboyle", Parola));
        var yanlisParola = await servis.LoginAsync(new LoginDto("sefa", "YanlisParola1"));

        // Kayıtlı kullanıcı adları dışarıdan anlaşılmamalı.
        Assert.Equal(yanlisParola.Errors[0].Message, olmayan.Errors[0].Message);
    }

    [Fact]
    public async Task LoginAsync_Bos_Alanlarda_Dogrulama_Hatasi_Dondurur()
    {
        using var scope = YeniIstek();

        var sonuc = await Servis<IAuthService>(scope).LoginAsync(new LoginDto(string.Empty, string.Empty));

        Assert.True(sonuc.IsFailure);
        Assert.Equal(ResultStatus.ValidationError, sonuc.Status);
        Assert.Equal(2, sonuc.Errors.Count);
    }

    [Fact]
    public async Task LoginAsync_Pasif_Kullanicida_Forbidden_Dondurur()
    {
        var kullaniciId = await KullaniciHazirlaAsync();

        using (var scope = YeniIstek())
        {
            await Servis<IUserService>(scope).UpdateAsync(
                new UserUpdateDto(kullaniciId, "sefa", "sefatayaz@gmail.com", "Muhammed Sefa", "Tayaz", null, false));
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IAuthService>(scope).LoginAsync(new LoginDto("sefa", Parola));

            Assert.True(sonuc.IsFailure);
            Assert.Equal(ResultStatus.Forbidden, sonuc.Status);
        }
    }

    [Fact]
    public async Task LoginAsync_Rolsuz_Kullanici_Yetki_Claimi_Almaz()
    {
        await KullaniciHazirlaAsync(rolAta: false);

        using var scope = YeniIstek();

        var sonuc = await Servis<IAuthService>(scope).LoginAsync(new LoginDto("sefa", Parola));

        Assert.True(sonuc.IsSuccess);
        Assert.Empty(sonuc.Data!.FindAll(AuthConstants.PermissionClaimType));
        Assert.Empty(sonuc.Data.FindAll(ClaimTypes.Role));
    }

    [Fact]
    public async Task LoginAsync_Son_Giris_Zamanini_Kaydeder()
    {
        var kullaniciId = await KullaniciHazirlaAsync();

        using (var scope = YeniIstek())
        {
            Assert.Null((await Servis<IUserService>(scope).GetByIdAsync(kullaniciId)).Data!.LastLoginAt);
        }

        using (var scope = YeniIstek())
        {
            Assert.True((await Servis<IAuthService>(scope).LoginAsync(new LoginDto("sefa", Parola))).IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            Assert.NotNull((await Servis<IUserService>(scope).GetByIdAsync(kullaniciId)).Data!.LastLoginAt);
        }
    }
}
