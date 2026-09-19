using Microsoft.Extensions.DependencyInjection;
using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Core.Entities;
using Portfolyo.Web.Core.Enums;
using Portfolyo.Web.Infrastructure.Extensions;

namespace Portfolyo.Web.Tests.Persistence;

/// <summary>
/// Repository + UnitOfWork zincirinin gerçekten diske yazıp okuduğunu doğrular.
/// Her test kendi geçici klasöründe çalışır.
/// </summary>
public class JsonPersistenceTests : IDisposable
{
    private readonly string _dataPath;
    private readonly ServiceProvider _provider;

    public JsonPersistenceTests()
    {
        _dataPath = Path.Combine(Path.GetTempPath(), "portfolyo-test-" + Guid.NewGuid().ToString("N"));

        var services = new ServiceCollection();
        services.AddInfrastructure(_dataPath);
        _provider = services.BuildServiceProvider();
    }

    /// <summary>Her çağrı yeni bir scope açar; yeni bir istek gibi davranır.</summary>
    private IServiceScope YeniIstek() => _provider.CreateScope();

    private static IUnitOfWork UnitOfWork(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

    [Fact]
    public async Task SaveChangesAsync_Kaydi_Diske_Yazar()
    {
        using (var scope = YeniIstek())
        {
            var unitOfWork = UnitOfWork(scope);
            await unitOfWork.Repository<User>().AddAsync(new User
            {
                UserName = "sefa",
                Email = "sefatayaz@gmail.com",
                FirstName = "Muhammed Sefa",
                LastName = "Tayaz",
                PasswordHash = "hash"
            });

            Assert.True(unitOfWork.HasChanges);
            Assert.Equal(1, await unitOfWork.SaveChangesAsync());
            Assert.False(unitOfWork.HasChanges);
        }

        Assert.True(File.Exists(Path.Combine(_dataPath, "users.json")));

        // Ayrı bir istek, veriyi dosyadan okuyabiliyor mu?
        using (var scope = YeniIstek())
        {
            var kullanicilar = await UnitOfWork(scope).Repository<User>().GetAllAsync();

            var kullanici = Assert.Single(kullanicilar);
            Assert.Equal("sefa", kullanici.UserName);
            Assert.True(kullanici.IsActive);
            Assert.NotEqual(Guid.Empty, kullanici.Id);
        }
    }

    [Fact]
    public async Task SaveChangesAsync_Cagrilmazsa_Veri_Diske_Yazilmaz()
    {
        using (var scope = YeniIstek())
        {
            await UnitOfWork(scope).Repository<User>().AddAsync(new User { UserName = "kaydedilmeyen" });
        }

        Assert.False(File.Exists(Path.Combine(_dataPath, "users.json")));

        using var yeniScope = YeniIstek();
        Assert.Empty(await UnitOfWork(yeniScope).Repository<User>().GetAllAsync());
    }

    [Fact]
    public async Task DiscardChanges_Kaydedilmemis_Degisiklikleri_Atar()
    {
        using var scope = YeniIstek();
        var unitOfWork = UnitOfWork(scope);

        await unitOfWork.Repository<User>().AddAsync(new User { UserName = "atilacak" });
        unitOfWork.DiscardChanges();

        Assert.False(unitOfWork.HasChanges);
        Assert.Empty(await unitOfWork.Repository<User>().GetAllAsync());
        Assert.Equal(0, await unitOfWork.SaveChangesAsync());
    }

    [Fact]
    public async Task SaveChangesAsync_Birden_Cok_Dosyayi_Ayni_Cagrida_Yazar()
    {
        var rolId = Guid.NewGuid();

        using (var scope = YeniIstek())
        {
            var unitOfWork = UnitOfWork(scope);

            await unitOfWork.Repository<Role>().AddAsync(new Role
            {
                Id = rolId,
                Name = "Yönetici",
                NormalizedName = "yonetici"
            });

            await unitOfWork.Repository<RolePermission>().AddRangeAsync(new[]
            {
                new RolePermission { RoleId = rolId, Permission = Permission.UserView },
                new RolePermission { RoleId = rolId, Permission = Permission.RoleView }
            });

            // İki farklı dosya değişti, tek çağrıda ikisi de yazılmalı.
            Assert.Equal(2, await unitOfWork.SaveChangesAsync());
        }

        Assert.True(File.Exists(Path.Combine(_dataPath, "roles.json")));
        Assert.True(File.Exists(Path.Combine(_dataPath, "rolePermissions.json")));

        using var yeniScope = YeniIstek();
        var yetkiler = await UnitOfWork(yeniScope).Repository<RolePermission>()
            .GetWhereAsync(x => x.RoleId == rolId);

        Assert.Equal(2, yetkiler.Count);
        Assert.Contains(yetkiler, x => x.Permission == Permission.UserView);
    }

    [Fact]
    public async Task UpdateAsync_CreatedAt_Korur_UpdatedAt_Atar()
    {
        Guid id;
        DateTime olusturma;

        using (var scope = YeniIstek())
        {
            var unitOfWork = UnitOfWork(scope);
            var rol = new Role { Name = "Editör", NormalizedName = "editor" };

            await unitOfWork.Repository<Role>().AddAsync(rol);
            await unitOfWork.SaveChangesAsync();

            id = rol.Id;
            olusturma = rol.CreatedAt;
        }

        using (var scope = YeniIstek())
        {
            var unitOfWork = UnitOfWork(scope);
            await unitOfWork.Repository<Role>().UpdateAsync(new Role
            {
                Id = id,
                Name = "İçerik Editörü",
                NormalizedName = "icerik-editoru",
                CreatedAt = DateTime.UtcNow // Yanlış değer gönderilse bile korunmalı.
            });

            await unitOfWork.SaveChangesAsync();
        }

        using var yeniScope = YeniIstek();
        var guncel = await UnitOfWork(yeniScope).Repository<Role>().GetByIdAsync(id);

        Assert.NotNull(guncel);
        Assert.Equal("İçerik Editörü", guncel!.Name);
        Assert.Equal(olusturma, guncel.CreatedAt, TimeSpan.FromMilliseconds(1));
        Assert.NotNull(guncel.UpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_Kaydi_Siler_Yoksa_False_Doner()
    {
        var rol = new Role { Name = "Silinecek", NormalizedName = "silinecek" };

        using (var scope = YeniIstek())
        {
            var unitOfWork = UnitOfWork(scope);
            await unitOfWork.Repository<Role>().AddAsync(rol);
            await unitOfWork.SaveChangesAsync();
        }

        using (var scope = YeniIstek())
        {
            var unitOfWork = UnitOfWork(scope);
            var repository = unitOfWork.Repository<Role>();

            Assert.False(await repository.DeleteAsync(Guid.NewGuid()));
            Assert.True(await repository.DeleteAsync(rol.Id));
            await unitOfWork.SaveChangesAsync();
        }

        using var yeniScope = YeniIstek();
        Assert.Empty(await UnitOfWork(yeniScope).Repository<Role>().GetAllAsync());
    }

    [Fact]
    public async Task Ayni_Istek_Icinde_Repository_Ayni_Ornegi_Doner()
    {
        using var scope = YeniIstek();
        var unitOfWork = UnitOfWork(scope);

        Assert.Same(unitOfWork.Repository<User>(), unitOfWork.Repository<User>());
    }

    public void Dispose()
    {
        _provider.Dispose();

        if (Directory.Exists(_dataPath))
        {
            Directory.Delete(_dataPath, recursive: true);
        }
    }
}
