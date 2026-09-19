using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.RolePermissions;
using Portfolyo.Web.Application.DTOs.Roles;
using Portfolyo.Web.Application.DTOs.UserRoles;
using Portfolyo.Web.Application.DTOs.Users;
using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Tests.Services;

/// <summary>
/// Atama servislerinin generic servisten miras aldığı CRUD metotları.
/// Kendi metotlarını yazmadan Create/Update/Delete/GetAll çalışmalı,
/// ama entity'ye özel kurallar (tekillik, var olmayan kayıt) yine devreye girmeli.
/// </summary>
public class InheritedCrudTests : ServiceTestBase
{
    private async Task<(Guid kullaniciId, Guid rolId)> VeriHazirlaAsync()
    {
        using var scope = YeniIstek();

        var kullanici = await Servis<IUserService>(scope).CreateAsync(
            new UserCreateDto("sefa", "sefatayaz@gmail.com", "Muhammed Sefa", "Tayaz", "Parola1234"));

        var rol = await Servis<IRoleService>(scope).CreateAsync(new RoleCreateDto("Yönetici", null));

        return (kullanici.Data!.Id, rol.Data!.Id);
    }

    [Fact]
    public async Task UserRole_Miras_Alinan_CreateAsync_Tek_Atama_Ekler()
    {
        var (kullaniciId, rolId) = await VeriHazirlaAsync();
        Guid atamaId;

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserRoleService>(scope)
                .CreateAsync(new UserRoleCreateDto(kullaniciId, rolId));

            Assert.True(sonuc.IsSuccess);
            Assert.Equal(kullaniciId, sonuc.Data!.UserId);
            Assert.Equal(rolId, sonuc.Data.RoleId);
            Assert.NotEqual(default, sonuc.Data.AssignedAt);

            atamaId = sonuc.Data.Id;
        }

        // Miras alınan GetAllAsync ve GetByIdAsync da çalışmalı.
        using (var scope = YeniIstek())
        {
            var servis = Servis<IUserRoleService>(scope);

            Assert.Single((await servis.GetAllAsync()).Data!);
            Assert.True((await servis.GetByIdAsync(atamaId)).IsSuccess);
            Assert.Equal(1, (await servis.CountAsync()).Data);
            Assert.True((await servis.ExistsAsync(atamaId)).Data);
        }

        // Miras alınan DeleteAsync.
        using (var scope = YeniIstek())
        {
            Assert.True((await Servis<IUserRoleService>(scope).DeleteAsync(atamaId)).IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            Assert.Empty((await Servis<IUserRoleService>(scope).GetAllAsync()).Data!);
        }
    }

    [Fact]
    public async Task UserRole_Ayni_Cift_Ikinci_Kez_Eklenirse_Conflict_Dondurur()
    {
        var (kullaniciId, rolId) = await VeriHazirlaAsync();

        using (var scope = YeniIstek())
        {
            Assert.True((await Servis<IUserRoleService>(scope)
                .CreateAsync(new UserRoleCreateDto(kullaniciId, rolId))).IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserRoleService>(scope)
                .CreateAsync(new UserRoleCreateDto(kullaniciId, rolId));

            Assert.True(sonuc.IsFailure);
            Assert.Equal(ResultStatus.Conflict, sonuc.Status);
            Assert.Equal("Bu rol kullanıcıya zaten atanmış.", sonuc.Message);
        }

        using (var scope = YeniIstek())
        {
            Assert.Single((await Servis<IUserRoleService>(scope).GetAllAsync()).Data!);
        }
    }

    [Fact]
    public async Task UserRole_Olmayan_Kullanici_Veya_Rolde_NotFound_Dondurur()
    {
        var (kullaniciId, rolId) = await VeriHazirlaAsync();

        using var scope = YeniIstek();
        var servis = Servis<IUserRoleService>(scope);

        var olmayanKullanici = await servis.CreateAsync(new UserRoleCreateDto(Guid.NewGuid(), rolId));
        var olmayanRol = await servis.CreateAsync(new UserRoleCreateDto(kullaniciId, Guid.NewGuid()));

        Assert.Equal(ResultStatus.NotFound, olmayanKullanici.Status);
        Assert.Equal("Kullanıcı bulunamadı.", olmayanKullanici.Message);
        Assert.Equal(ResultStatus.NotFound, olmayanRol.Status);
        Assert.Equal("Rol bulunamadı.", olmayanRol.Message);
    }

    [Fact]
    public async Task UserRole_Bos_Id_ile_ValidationError_Dondurur()
    {
        using var scope = YeniIstek();

        var sonuc = await Servis<IUserRoleService>(scope)
            .CreateAsync(new UserRoleCreateDto(Guid.Empty, Guid.Empty));

        Assert.True(sonuc.IsFailure);
        Assert.Equal(ResultStatus.ValidationError, sonuc.Status);
        Assert.Equal(2, sonuc.Errors.Count);
    }

    [Fact]
    public async Task RolePermission_Miras_Alinan_CreateAsync_Ve_UpdateAsync_Calisir()
    {
        var (_, rolId) = await VeriHazirlaAsync();
        Guid kayitId;

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IRolePermissionService>(scope)
                .CreateAsync(new RolePermissionCreateDto(rolId, Permission.UserView));

            Assert.True(sonuc.IsSuccess);
            Assert.Equal(Permission.UserView, sonuc.Data!.Permission);

            kayitId = sonuc.Data.Id;
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IRolePermissionService>(scope)
                .UpdateAsync(new RolePermissionUpdateDto(kayitId, rolId, Permission.RoleDelete));

            Assert.True(sonuc.IsSuccess);
            Assert.Equal(Permission.RoleDelete, sonuc.Data!.Permission);
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IRolePermissionService>(scope).GetByRoleAsync(rolId);

            var yetki = Assert.Single(sonuc.Data!);
            Assert.Equal(Permission.RoleDelete, yetki.Permission);
        }
    }

    [Fact]
    public async Task RolePermission_Ayni_Yetki_Ikinci_Kez_Verilirse_Conflict_Dondurur()
    {
        var (_, rolId) = await VeriHazirlaAsync();

        using (var scope = YeniIstek())
        {
            Assert.True((await Servis<IRolePermissionService>(scope)
                .CreateAsync(new RolePermissionCreateDto(rolId, Permission.UserView))).IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IRolePermissionService>(scope)
                .CreateAsync(new RolePermissionCreateDto(rolId, Permission.UserView));

            Assert.True(sonuc.IsFailure);
            Assert.Equal(ResultStatus.Conflict, sonuc.Status);
            Assert.Equal("Bu yetki role zaten verilmiş.", sonuc.Message);
        }
    }

    [Fact]
    public async Task RolePermission_Tanimsiz_Yetkide_ValidationError_Dondurur()
    {
        var (_, rolId) = await VeriHazirlaAsync();

        using var scope = YeniIstek();

        var sonuc = await Servis<IRolePermissionService>(scope)
            .CreateAsync(new RolePermissionCreateDto(rolId, (Permission)9999));

        Assert.True(sonuc.IsFailure);
        Assert.Equal(ResultStatus.ValidationError, sonuc.Status);
        Assert.Contains(sonuc.Errors, hata => hata.PropertyName == "Permission");
    }

    [Fact]
    public async Task Toplu_Atama_Ve_Miras_Alinan_Crud_Ayni_Veriyi_Gorur()
    {
        var (kullaniciId, rolId) = await VeriHazirlaAsync();

        // Toplu atama ile eklenen kayıt, miras alınan GetAllAsync ile de görünmeli.
        using (var scope = YeniIstek())
        {
            Assert.True((await Servis<IUserRoleService>(scope)
                .AssignAsync(new UserRoleAssignDto(kullaniciId, new[] { rolId }))).IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var servis = Servis<IUserRoleService>(scope);

            var hepsi = await servis.GetAllAsync();
            var atama = Assert.Single(hepsi.Data!);

            Assert.Equal(rolId, atama.RoleId);

            // Tersi de geçerli: miras alınan DeleteAsync sonrası toplu sorgu boş dönmeli.
            Assert.True((await servis.DeleteAsync(atama.Id)).IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            Assert.Empty((await Servis<IUserRoleService>(scope).GetRoleIdsAsync(kullaniciId)).Data!);
        }
    }
}
