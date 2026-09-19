using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common;
using Portfolyo.Web.Application.DTOs.Roles;
using Portfolyo.Web.Application.DTOs.Users;
using Portfolyo.Web.Application.Extensions;
using Portfolyo.Web.Core.Entities;
using Portfolyo.Web.Infrastructure.Extensions;

namespace Portfolyo.Web.Tests.Services;

/// <summary>
/// Generic servisin doğrulama, eşleme ve kaydetme zincirini doğrular.
/// </summary>
public class GenericServiceTests : IDisposable
{
    private readonly string _dataPath;
    private readonly ServiceProvider _provider;

    public GenericServiceTests()
    {
        _dataPath = Path.Combine(Path.GetTempPath(), "portfolyo-test-" + Guid.NewGuid().ToString("N"));

        var services = new ServiceCollection();
        services.AddLogging(); // AutoMapper ILoggerFactory bekliyor; web tarafında hazır geliyor.
        services.AddInfrastructure(_dataPath);
        services.AddApplication();
        _provider = services.BuildServiceProvider();
    }

    private IServiceScope YeniIstek() => _provider.CreateScope();

    private static IService<Role, RoleDto, RoleCreateDto, RoleUpdateDto> RolServisi(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IService<Role, RoleDto, RoleCreateDto, RoleUpdateDto>>();

    private static IService<User, UserDto, UserCreateDto, UserUpdateDto> KullaniciServisi(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IService<User, UserDto, UserCreateDto, UserUpdateDto>>();

    [Fact]
    public async Task CreateAsync_Kaydi_Olusturur_Ve_NormalizedName_Uretir()
    {
        RoleDto olusan;

        using (var scope = YeniIstek())
        {
            olusan = await RolServisi(scope).CreateAsync(new RoleCreateDto("İçerik Editörü", "Blog yazılarını yönetir"));
        }

        Assert.Equal("İçerik Editörü", olusan.Name);
        Assert.Equal("icerik-editoru", olusan.NormalizedName);
        Assert.NotEqual(Guid.Empty, olusan.Id);

        // Servis kendi SaveChangesAsync çağrısını yaptığı için kayıt diskte olmalı.
        Assert.True(File.Exists(Path.Combine(_dataPath, "roles.json")));

        using var yeniScope = YeniIstek();
        var kayitli = await RolServisi(yeniScope).GetByIdAsync(olusan.Id);

        Assert.NotNull(kayitli);
        Assert.Equal("icerik-editoru", kayitli!.NormalizedName);
    }

    [Fact]
    public async Task CreateAsync_Gecersiz_Girdide_ValidationException_Firlatir()
    {
        using var scope = YeniIstek();

        var hata = await Assert.ThrowsAsync<ValidationException>(
            () => RolServisi(scope).CreateAsync(new RoleCreateDto("", null)));

        Assert.Contains("Rol adı zorunludur.", hata.Message);

        // Doğrulama başarısızsa hiçbir şey kaydedilmemeli.
        Assert.False(File.Exists(Path.Combine(_dataPath, "roles.json")));
    }

    [Fact]
    public async Task UpdateAsync_Dto_Disindaki_Alanlari_Korur()
    {
        Guid id;
        DateTime olusturma;

        using (var scope = YeniIstek())
        {
            // Parola alanı DTO üzerinden gelmediği için doğrudan repository ile yazılıyor.
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var kullanici = new User
            {
                UserName = "sefa",
                Email = "sefatayaz@gmail.com",
                FirstName = "Muhammed Sefa",
                LastName = "Tayaz",
                PasswordHash = "gizli-hash",
                LastLoginAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            await unitOfWork.Repository<User>().AddAsync(kullanici);
            await unitOfWork.SaveChangesAsync();

            id = kullanici.Id;
            olusturma = kullanici.CreatedAt;
        }

        using (var scope = YeniIstek())
        {
            var guncellenen = await KullaniciServisi(scope).UpdateAsync(
                new UserUpdateDto(id, "sefa", "yeni@mail.com", "Sefa", "Tayaz", null, false));

            Assert.Equal("yeni@mail.com", guncellenen.Email);
            Assert.False(guncellenen.IsActive);
        }

        using (var scope = YeniIstek())
        {
            // DTO'da olmayan alanlar ezilmemiş olmalı.
            var kullanici = await scope.ServiceProvider.GetRequiredService<IUnitOfWork>()
                .Repository<User>().GetByIdAsync(id);

            Assert.NotNull(kullanici);
            Assert.Equal("gizli-hash", kullanici!.PasswordHash);
            Assert.NotNull(kullanici.LastLoginAt);
            Assert.Equal(olusturma, kullanici.CreatedAt, TimeSpan.FromMilliseconds(1));
            Assert.NotNull(kullanici.UpdatedAt);
        }
    }

    [Fact]
    public async Task UpdateAsync_Kayit_Yoksa_NotFoundException_Firlatir()
    {
        using var scope = YeniIstek();

        await Assert.ThrowsAsync<Portfolyo.Web.Application.Common.Exceptions.NotFoundException>(
            () => RolServisi(scope).UpdateAsync(new RoleUpdateDto(Guid.NewGuid(), "Yönetici", null)));
    }

    [Fact]
    public async Task CreateAsync_Parolayi_Duz_Metin_Olarak_Kaydetmez()
    {
        UserDto olusan;

        using (var scope = YeniIstek())
        {
            olusan = await KullaniciServisi(scope).CreateAsync(
                new UserCreateDto("sefa", "sefatayaz@gmail.com", "Muhammed Sefa", "Tayaz", "Parola1234"));
        }

        using var yeniScope = YeniIstek();
        var kullanici = await yeniScope.ServiceProvider.GetRequiredService<IUnitOfWork>()
            .Repository<User>().GetByIdAsync(olusan.Id);

        Assert.NotNull(kullanici);
        Assert.DoesNotContain("Parola1234", kullanici!.PasswordHash);

        // Dosyanın hiçbir yerinde düz parola geçmemeli.
        var icerik = await File.ReadAllTextAsync(Path.Combine(_dataPath, "users.json"));
        Assert.DoesNotContain("Parola1234", icerik);
    }

    [Fact]
    public async Task DeleteAsync_Kaydi_Siler_Yoksa_False_Doner()
    {
        Guid id;

        using (var scope = YeniIstek())
        {
            var olusan = await RolServisi(scope).CreateAsync(new RoleCreateDto("Silinecek", null));
            id = olusan.Id;
        }

        using (var scope = YeniIstek())
        {
            var servis = RolServisi(scope);

            Assert.False(await servis.DeleteAsync(Guid.NewGuid()));
            Assert.True(await servis.DeleteAsync(id));
        }

        using var yeniScope = YeniIstek();
        Assert.Empty(await RolServisi(yeniScope).GetAllAsync());
    }

    [Fact]
    public async Task GetAllAsync_Ve_CountAsync_Dto_Dondurur()
    {
        using (var scope = YeniIstek())
        {
            var servis = RolServisi(scope);
            await servis.CreateAsync(new RoleCreateDto("Yönetici", null));
            await servis.CreateAsync(new RoleCreateDto("Editör", null));
        }

        using var yeniScope = YeniIstek();
        var servis2 = RolServisi(yeniScope);

        var roller = await servis2.GetAllAsync();

        Assert.Equal(2, roller.Count);
        Assert.Equal(2, await servis2.CountAsync());
        Assert.All(roller, rol => Assert.IsType<RoleDto>(rol));

        var filtreli = await servis2.GetWhereAsync(rol => rol.NormalizedName == "editor");
        Assert.Single(filtreli);
    }

    [Fact]
    public void TextNormalizer_Turkce_Karakterleri_Cevirir()
    {
        Assert.Equal("icerik-editoru", TextNormalizer.Normalize("İçerik Editörü"));
        Assert.Equal("super-yonetici", TextNormalizer.Normalize("  Süper   Yönetici  "));
        Assert.Equal("sifir-gun", TextNormalizer.Normalize("Sıfır/Gün"));
        Assert.Equal(string.Empty, TextNormalizer.Normalize(null));
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
