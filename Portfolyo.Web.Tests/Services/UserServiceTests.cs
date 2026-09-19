using Microsoft.Extensions.DependencyInjection;
using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.Users;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Tests.Services;

public class UserServiceTests : ServiceTestBase
{
    private static UserCreateDto GecerliKullanici(string userName = "sefa", string email = "sefatayaz@gmail.com")
        => new(userName, email, "Muhammed Sefa", "Tayaz", "Parola1234");

    [Fact]
    public async Task CreateAsync_Parolayi_Hashler_Ve_Duz_Metin_Saklamaz()
    {
        Guid id;

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserService>(scope).CreateAsync(GecerliKullanici());

            Assert.True(sonuc.IsSuccess);
            id = sonuc.Data!.Id;
        }

        using (var scope = YeniIstek())
        {
            var kullanici = await scope.ServiceProvider.GetRequiredService<IUnitOfWork>()
                .Repository<User>().GetByIdAsync(id);

            Assert.NotNull(kullanici);
            Assert.NotEmpty(kullanici!.PasswordHash);
            Assert.DoesNotContain("Parola1234", kullanici.PasswordHash);

            // Hash gerçekten doğrulanabilir olmalı.
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            Assert.True(hasher.Verify(kullanici.PasswordHash, "Parola1234"));
            Assert.False(hasher.Verify(kullanici.PasswordHash, "YanlisParola1"));
        }

        var icerik = await DosyaOkuAsync("users.json");
        Assert.DoesNotContain("Parola1234", icerik);
    }

    [Fact]
    public async Task CreateAsync_Ayni_Kullanici_Adi_Varsa_Conflict_Dondurur()
    {
        using (var scope = YeniIstek())
        {
            Assert.True((await Servis<IUserService>(scope).CreateAsync(GecerliKullanici())).IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserService>(scope)
                .CreateAsync(GecerliKullanici("SEFA", "baska@mail.com"));

            Assert.True(sonuc.IsFailure);
            Assert.Equal(ResultStatus.Conflict, sonuc.Status);
            Assert.Equal("Bu kullanıcı adı zaten kullanılıyor.", sonuc.Message);
        }
    }

    [Fact]
    public async Task CreateAsync_Ayni_Eposta_Varsa_Conflict_Dondurur()
    {
        using (var scope = YeniIstek())
        {
            Assert.True((await Servis<IUserService>(scope).CreateAsync(GecerliKullanici())).IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserService>(scope)
                .CreateAsync(GecerliKullanici("baska", "SEFATAYAZ@gmail.com"));

            Assert.True(sonuc.IsFailure);
            Assert.Equal(ResultStatus.Conflict, sonuc.Status);
            Assert.Equal("Bu e-posta adresi zaten kullanılıyor.", sonuc.Message);
        }
    }

    [Fact]
    public async Task CreateAsync_Kisa_Parolada_ValidationError_Dondurur()
    {
        using var scope = YeniIstek();

        var sonuc = await Servis<IUserService>(scope)
            .CreateAsync(new UserCreateDto("sefa", "sefa@mail.com", "Sefa", "Tayaz", "kisa"));

        Assert.True(sonuc.IsFailure);
        Assert.Equal(ResultStatus.ValidationError, sonuc.Status);
        Assert.Contains(sonuc.Errors, hata => hata.PropertyName == "Password");
        Assert.False(DosyaVar("users.json"));
    }

    [Fact]
    public async Task UpdateAsync_Parolayi_Ve_Olusturma_Tarihini_Korur()
    {
        Guid id;
        string hash;
        DateTime olusturma;

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserService>(scope).CreateAsync(GecerliKullanici());
            id = sonuc.Data!.Id;
            olusturma = sonuc.Data.CreatedAt;
        }

        using (var scope = YeniIstek())
        {
            hash = (await scope.ServiceProvider.GetRequiredService<IUnitOfWork>()
                .Repository<User>().GetByIdAsync(id))!.PasswordHash;
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserService>(scope).UpdateAsync(
                new UserUpdateDto(id, "sefa", "yeni@mail.com", "Sefa", "Tayaz", null, false));

            Assert.True(sonuc.IsSuccess);
            Assert.Equal("yeni@mail.com", sonuc.Data!.Email);
            Assert.False(sonuc.Data.IsActive);
        }

        using (var scope = YeniIstek())
        {
            var kullanici = await scope.ServiceProvider.GetRequiredService<IUnitOfWork>()
                .Repository<User>().GetByIdAsync(id);

            Assert.Equal(hash, kullanici!.PasswordHash);
            Assert.Equal(olusturma, kullanici.CreatedAt, TimeSpan.FromMilliseconds(1));
            Assert.NotNull(kullanici.UpdatedAt);
        }
    }

    [Fact]
    public async Task ChangePasswordAsync_Mevcut_Parola_Yanlissa_ValidationError_Dondurur()
    {
        Guid id;

        using (var scope = YeniIstek())
        {
            id = (await Servis<IUserService>(scope).CreateAsync(GecerliKullanici())).Data!.Id;
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserService>(scope)
                .ChangePasswordAsync(id, "YanlisParola1", "YeniParola123");

            Assert.True(sonuc.IsFailure);
            Assert.Equal(ResultStatus.ValidationError, sonuc.Status);
            Assert.Equal("Mevcut parola hatalı.", sonuc.Errors[0].Message);
        }

        // Parola değişmemiş olmalı.
        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserService>(scope).VerifyPasswordAsync("sefa", "Parola1234");
            Assert.True(sonuc.IsSuccess);
        }
    }

    [Fact]
    public async Task ChangePasswordAsync_Dogru_Parolayla_Yeni_Parolayi_Kaydeder()
    {
        Guid id;

        using (var scope = YeniIstek())
        {
            id = (await Servis<IUserService>(scope).CreateAsync(GecerliKullanici())).Data!.Id;
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserService>(scope)
                .ChangePasswordAsync(id, "Parola1234", "YeniParola123");

            Assert.True(sonuc.IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var servis = Servis<IUserService>(scope);

            Assert.True((await servis.VerifyPasswordAsync("sefa", "YeniParola123")).IsSuccess);
            Assert.True((await servis.VerifyPasswordAsync("sefa", "Parola1234")).IsFailure);
        }
    }

    [Fact]
    public async Task VerifyPasswordAsync_Olmayan_Kullanici_Ve_Yanlis_Parola_Ayni_Mesaji_Doner()
    {
        using (var scope = YeniIstek())
        {
            await Servis<IUserService>(scope).CreateAsync(GecerliKullanici());
        }

        using var yeniScope = YeniIstek();
        var servis = Servis<IUserService>(yeniScope);

        var olmayanKullanici = await servis.VerifyPasswordAsync("yokboyle", "Parola1234");
        var yanlisParola = await servis.VerifyPasswordAsync("sefa", "YanlisParola1");

        // Hangi kullanıcı adlarının kayıtlı olduğu dışarıdan anlaşılmamalı.
        Assert.Equal(ResultStatus.ValidationError, olmayanKullanici.Status);
        Assert.Equal(ResultStatus.ValidationError, yanlisParola.Status);
        Assert.Equal(olmayanKullanici.Errors[0].Message, yanlisParola.Errors[0].Message);
    }

    [Fact]
    public async Task VerifyPasswordAsync_Pasif_Kullanicida_Forbidden_Dondurur()
    {
        Guid id;

        using (var scope = YeniIstek())
        {
            id = (await Servis<IUserService>(scope).CreateAsync(GecerliKullanici())).Data!.Id;
        }

        using (var scope = YeniIstek())
        {
            await Servis<IUserService>(scope).UpdateAsync(
                new UserUpdateDto(id, "sefa", "sefatayaz@gmail.com", "Sefa", "Tayaz", null, false));
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserService>(scope).VerifyPasswordAsync("sefa", "Parola1234");

            Assert.True(sonuc.IsFailure);
            Assert.Equal(ResultStatus.Forbidden, sonuc.Status);
        }
    }

    [Fact]
    public async Task GetByUserNameAsync_Buyuk_Kucuk_Harf_Ayrimi_Yapmaz()
    {
        using (var scope = YeniIstek())
        {
            await Servis<IUserService>(scope).CreateAsync(GecerliKullanici());
        }

        using var yeniScope = YeniIstek();
        var sonuc = await Servis<IUserService>(yeniScope).GetByUserNameAsync("SeFa");

        Assert.True(sonuc.IsSuccess);
        Assert.Equal("Muhammed Sefa Tayaz", sonuc.Data!.FullName);
    }

    [Fact]
    public async Task RecordLoginAsync_Son_Giris_Zamanini_Yazar()
    {
        Guid id;

        using (var scope = YeniIstek())
        {
            id = (await Servis<IUserService>(scope).CreateAsync(GecerliKullanici())).Data!.Id;
        }

        using (var scope = YeniIstek())
        {
            Assert.True((await Servis<IUserService>(scope).RecordLoginAsync(id)).IsSuccess);
        }

        using (var scope = YeniIstek())
        {
            var sonuc = await Servis<IUserService>(scope).GetByIdAsync(id);
            Assert.NotNull(sonuc.Data!.LastLoginAt);
        }
    }
}
