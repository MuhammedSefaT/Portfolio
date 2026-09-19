using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.RolePermissions;
using Portfolyo.Web.Application.DTOs.Roles;
using Portfolyo.Web.Core.Entities;
using Portfolyo.Web.Core.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Portfolyo.Web.Tests.Services;

public class RoleServiceTests : ServiceTestBase
{
    [Fact]
    public async Task CreateAsync_Basarili_Sonuc_Ve_NormalizedName_Dondurur()
    {
        using var scope = YeniIstek();

        var sonuc = await Servis<IRoleService>(scope)
            .CreateAsync(new RoleCreateDto("İçerik Editörü", "Blog yazılarını yönetir"));

        Assert.True(sonuc.IsSuccess);
        Assert.Equal(ResultStatus.Success, sonuc.Status);
        Assert.NotNull(sonuc.Data);
        Assert.Equal("icerik-editoru", sonuc.Data!.NormalizedName);
        Assert.Empty(sonuc.Errors);
    }

    [Fact]
    public async Task CreateAsync_Gecersiz_Girdide_ValidationError_Dondurur()
    {
        using var scope = YeniIstek();

        var sonuc = await Servis<IRoleService>(scope).CreateAsync(new RoleCreateDto("", null));

        Assert.True(sonuc.IsFailure);
        Assert.Equal(ResultStatus.ValidationError, sonuc.Status);
        Assert.Null(sonuc.Data);
        Assert.Contains(sonuc.Errors, hata => hata.PropertyName == "Name");
        Assert.Contains(sonuc.Errors, hata => hata.Message == "Rol adı zorunludur.");

        // Doğrulama başarısızsa hiçbir şey diske yazılmamalı.
        Assert.False(DosyaVar("roles.json"));
    }

    [Fact]
    public async Task CreateAsync_Ayni_Isimde_Rol_Varsa_Conflict_Dondurur()
    {
        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IRoleService>(scope).CreateAsync(new RoleCreateDto("Yönetici", null));
            Assert.True(sonuc.IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            // Normalleştirme sonrası aynı: "yonetici"
            var sonuc = await Servis<IRoleService>(scope).CreateAsync(new RoleCreateDto("yönetici", null));

            Assert.True(sonuc.IsFailure);
            Assert.Equal(ResultStatus.Conflict, sonuc.Status);
            Assert.Equal("Bu isimde bir rol zaten var.", sonuc.Message);
        }

        using (var scope = YeniIstek())
        {
            var hepsi = await Servis<IRoleService>(scope).GetAllAsync();
            Assert.Single(hepsi.Data!);
        }
    }

    [Fact]
    public async Task GetByIdAsync_Kayit_Yoksa_NotFound_Dondurur()
    {
        using var scope = YeniIstek();

        var sonuc = await Servis<IRoleService>(scope).GetByIdAsync(Guid.NewGuid());

        Assert.True(sonuc.IsFailure);
        Assert.Equal(ResultStatus.NotFound, sonuc.Status);
        Assert.Equal("Rol bulunamadı.", sonuc.Message);
    }

    [Fact]
    public async Task UpdateAsync_Kayit_Yoksa_NotFound_Dondurur()
    {
        using var scope = YeniIstek();

        var sonuc = await Servis<IRoleService>(scope)
            .UpdateAsync(new RoleUpdateDto(Guid.NewGuid(), "Yönetici", null));

        Assert.True(sonuc.IsFailure);
        Assert.Equal(ResultStatus.NotFound, sonuc.Status);
    }

    [Fact]
    public async Task UpdateAsync_Baska_Rolun_Adiyla_Cakisirsa_Degisiklik_Diske_Yazilmaz()
    {
        Guid editorId;

        using (var scope = YeniIstek())
        {
            var servis = Servis<IRoleService>(scope);
            await servis.CreateAsync(new RoleCreateDto("Yönetici", null));
            editorId = (await servis.CreateAsync(new RoleCreateDto("Editör", null))).Data!.Id;
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IRoleService>(scope)
                .UpdateAsync(new RoleUpdateDto(editorId, "Yönetici", null));

            Assert.True(sonuc.IsFailure);
            Assert.Equal(ResultStatus.Conflict, sonuc.Status);
        }

        using (var scope = YeniIstek())
        {
            // Çakışma yüzünden iptal edilen güncelleme dosyaya sızmamalı.
            var sonuc = await Servis<IRoleService>(scope).GetByIdAsync(editorId);

            Assert.True(sonuc.IsSuccess);
            Assert.Equal("Editör", sonuc.Data!.Name);
            Assert.Equal("editor", sonuc.Data.NormalizedName);
        }
    }

    [Fact]
    public async Task DeleteAsync_Role_Bagli_Yetki_Ve_Atamalari_Da_Temizler()
    {
        Guid rolId;
        Guid kullaniciId;

        using (var scope = YeniIstek())
        {
            var rolServisi = Servis<IRoleService>(scope);
            rolId = (await rolServisi.CreateAsync(new RoleCreateDto("Yönetici", null))).Data!.Id;

            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var kullanici = new User { UserName = "sefa", Email = "sefa@mail.com", PasswordHash = "hash" };
            await unitOfWork.Repository<User>().AddAsync(kullanici);
            await unitOfWork.SaveChangesAsync();
            kullaniciId = kullanici.Id;

            await Servis<IRolePermissionService>(scope).AssignAsync(
                new RolePermissionAssignDto(rolId, new[] { Permission.UserView, Permission.RoleView }));

            await Servis<IUserRoleService>(scope).AssignAsync(
                new Portfolyo.Web.Application.DTOs.UserRoles.UserRoleAssignDto(kullaniciId, new[] { rolId }));
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IRoleService>(scope).DeleteAsync(rolId);
            Assert.True(sonuc.IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            Assert.Empty(await unitOfWork.Repository<Role>().GetAllAsync());
            Assert.Empty(await unitOfWork.Repository<RolePermission>().GetAllAsync());
            Assert.Empty(await unitOfWork.Repository<UserRole>().GetAllAsync());

            // Kullanıcı silinmemeli, yalnızca ataması kalkmalı.
            Assert.Single(await unitOfWork.Repository<User>().GetAllAsync());
        }
    }

    [Fact]
    public async Task GetDetailAsync_Rolu_Yetkileriyle_Dondurur()
    {
        Guid rolId;

        using (var scope = YeniIstek())
        {
            rolId = (await Servis<IRoleService>(scope).CreateAsync(new RoleCreateDto("Yönetici", null))).Data!.Id;

            await Servis<IRolePermissionService>(scope).AssignAsync(
                new RolePermissionAssignDto(rolId, new[] { Permission.RoleView, Permission.DashboardView }));
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IRoleService>(scope).GetDetailAsync(rolId);

            Assert.True(sonuc.IsSuccess);
            Assert.Equal(2, sonuc.Data!.Permissions.Count);
            Assert.Contains(Permission.DashboardView, sonuc.Data.Permissions);
            Assert.Contains(Permission.RoleView, sonuc.Data.Permissions);
        }
    }
}
