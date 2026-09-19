using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.RolePermissions;
using Portfolyo.Web.Application.DTOs.Roles;
using Portfolyo.Web.Application.DTOs.UserRoles;
using Portfolyo.Web.Application.DTOs.Users;
using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Tests.Services;

/// <summary>
/// Kullanıcı-rol ve rol-yetki atama servisleri.
/// </summary>
public class AssignmentServiceTests : ServiceTestBase
{
    private async Task<(Guid kullaniciId, Guid yoneticiRolId, Guid editorRolId)> VeriHazirlaAsync()
    {
        using var scope = YeniIstek();

        var kullanici = await Servis<IUserService>(scope).CreateAsync(
            new UserCreateDto("sefa", "sefatayaz@gmail.com", "Muhammed Sefa", "Tayaz", "Parola1234"));

        var rolServisi = Servis<IRoleService>(scope);
        var yonetici = await rolServisi.CreateAsync(new RoleCreateDto("Yönetici", null));
        var editor = await rolServisi.CreateAsync(new RoleCreateDto("Editör", null));

        return (kullanici.Data!.Id, yonetici.Data!.Id, editor.Data!.Id);
    }

    [Fact]
    public async Task AssignAsync_Rolleri_Son_Duruma_Gore_Ayarlar()
    {
        var (kullaniciId, yoneticiRolId, editorRolId) = await VeriHazirlaAsync();

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserRoleService>(scope)
                .AssignAsync(new UserRoleAssignDto(kullaniciId, new[] { yoneticiRolId, editorRolId }));

            Assert.True(sonuc.IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserRoleService>(scope).GetRoleIdsAsync(kullaniciId);
            Assert.Equal(2, sonuc.Data!.Count);
        }

        // Listeden çıkarılan rol kaldırılmalı, listede kalan korunmalı.
        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserRoleService>(scope)
                .AssignAsync(new UserRoleAssignDto(kullaniciId, new[] { editorRolId }));

            Assert.True(sonuc.IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserRoleService>(scope).GetRoleIdsAsync(kullaniciId);

            Assert.Single(sonuc.Data!);
            Assert.Equal(editorRolId, sonuc.Data![0]);
        }
    }

    [Fact]
    public async Task AssignAsync_Bos_Liste_Tum_Rolleri_Kaldirir()
    {
        var (kullaniciId, yoneticiRolId, _) = await VeriHazirlaAsync();

        using (var scope = YeniIstek())
        {
            await Servis<IUserRoleService>(scope)
                .AssignAsync(new UserRoleAssignDto(kullaniciId, new[] { yoneticiRolId }));
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserRoleService>(scope)
                .AssignAsync(new UserRoleAssignDto(kullaniciId, Array.Empty<Guid>()));

            Assert.True(sonuc.IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            Assert.Empty((await Servis<IUserRoleService>(scope).GetRoleIdsAsync(kullaniciId)).Data!);
        }
    }

    [Fact]
    public async Task AssignAsync_Olmayan_Rol_Gonderilirse_Hicbir_Sey_Degismez()
    {
        var (kullaniciId, yoneticiRolId, _) = await VeriHazirlaAsync();

        using (var scope = YeniIstek())
        {
            await Servis<IUserRoleService>(scope)
                .AssignAsync(new UserRoleAssignDto(kullaniciId, new[] { yoneticiRolId }));
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserRoleService>(scope)
                .AssignAsync(new UserRoleAssignDto(kullaniciId, new[] { Guid.NewGuid() }));

            Assert.True(sonuc.IsFailure);
            Assert.Equal(ResultStatus.ValidationError, sonuc.Status);
        }

        using (var scope = YeniIstek())
        {
            // Mevcut atama bozulmamalı.
            var sonuc = await Servis<IUserRoleService>(scope).GetRoleIdsAsync(kullaniciId);

            Assert.Single(sonuc.Data!);
            Assert.Equal(yoneticiRolId, sonuc.Data![0]);
        }
    }

    [Fact]
    public async Task AssignAsync_Olmayan_Kullanicida_NotFound_Dondurur()
    {
        using var scope = YeniIstek();

        var sonuc = await Servis<IUserRoleService>(scope)
            .AssignAsync(new UserRoleAssignDto(Guid.NewGuid(), Array.Empty<Guid>()));

        Assert.True(sonuc.IsFailure);
        Assert.Equal(ResultStatus.NotFound, sonuc.Status);
    }

    [Fact]
    public async Task GetByUserAsync_Rol_Adlarini_Birlestirir()
    {
        var (kullaniciId, yoneticiRolId, _) = await VeriHazirlaAsync();

        using (var scope = YeniIstek())
        {
            await Servis<IUserRoleService>(scope)
                .AssignAsync(new UserRoleAssignDto(kullaniciId, new[] { yoneticiRolId }));
        }

        using var yeniScope = YeniIstek();
        var sonuc = await Servis<IUserRoleService>(yeniScope).GetByUserAsync(kullaniciId);

        Assert.True(sonuc.IsSuccess);

        var atama = Assert.Single(sonuc.Data!);
        Assert.Equal("Yönetici", atama.RoleName);
        Assert.Equal("sefa", atama.UserName);
    }

    [Fact]
    public async Task RolePermission_AssignAsync_Yetkileri_Son_Duruma_Gore_Ayarlar()
    {
        var (_, yoneticiRolId, _) = await VeriHazirlaAsync();

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IRolePermissionService>(scope).AssignAsync(
                new RolePermissionAssignDto(yoneticiRolId,
                    new[] { Permission.UserView, Permission.UserCreate, Permission.RoleView }));

            Assert.True(sonuc.IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IRolePermissionService>(scope).AssignAsync(
                new RolePermissionAssignDto(yoneticiRolId, new[] { Permission.UserView }));

            Assert.True(sonuc.IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IRolePermissionService>(scope).GetByRoleAsync(yoneticiRolId);

            var yetki = Assert.Single(sonuc.Data!);
            Assert.Equal(Permission.UserView, yetki.Permission);
        }
    }

    [Fact]
    public async Task RolePermission_AssignAsync_Tanimsiz_Yetkide_ValidationError_Dondurur()
    {
        var (_, yoneticiRolId, _) = await VeriHazirlaAsync();

        using var scope = YeniIstek();

        var sonuc = await Servis<IRolePermissionService>(scope).AssignAsync(
            new RolePermissionAssignDto(yoneticiRolId, new[] { (Permission)9999 }));

        Assert.True(sonuc.IsFailure);
        Assert.Equal(ResultStatus.ValidationError, sonuc.Status);
    }

    [Fact]
    public async Task GetByUserAsync_Kullanicinin_Tum_Rollerinden_Gelen_Yetkileri_Birlestirir()
    {
        var (kullaniciId, yoneticiRolId, editorRolId) = await VeriHazirlaAsync();

        using (var scope = YeniIstek())
        {
            var yetkiServisi = Servis<IRolePermissionService>(scope);

            await yetkiServisi.AssignAsync(new RolePermissionAssignDto(
                yoneticiRolId, new[] { Permission.UserView, Permission.RoleView }));

            await yetkiServisi.AssignAsync(new RolePermissionAssignDto(
                editorRolId, new[] { Permission.ContentCreate, Permission.UserView }));

            await Servis<IUserRoleService>(scope)
                .AssignAsync(new UserRoleAssignDto(kullaniciId, new[] { yoneticiRolId, editorRolId }));
        }

        using var yeniScope = YeniIstek();
        var servis = Servis<IRolePermissionService>(yeniScope);

        var sonuc = await servis.GetByUserAsync(kullaniciId);

        Assert.True(sonuc.IsSuccess);

        // UserView iki rolde de var; tekilleştirilmeli.
        Assert.Equal(3, sonuc.Data!.Count);
        Assert.Contains(Permission.ContentCreate, sonuc.Data);

        Assert.True((await servis.UserHasPermissionAsync(kullaniciId, Permission.RoleView)).Data);
        Assert.False((await servis.UserHasPermissionAsync(kullaniciId, Permission.SettingUpdate)).Data);
    }
}
