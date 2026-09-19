using Microsoft.Extensions.DependencyInjection;
using Portfolyo.Web.Infrastructure.Persistence.Seed;

namespace Portfolyo.Web.Tests.Services;

/// <summary>
/// Seed kapalıyken hiçbir veri yazılmamalı.
/// </summary>
public class DataSeederDisabledTests : ServiceTestBase
{
    protected override void Yapilandir(IServiceCollection services)
        => services.Configure<SeedOptions>(options => options.Enabled = false);

    [Fact]
    public async Task Seed_Kapaliyken_Hicbir_Dosya_Yazilmaz()
    {
        using (var scope = YeniIstek())
        {
            Assert.True((await Servis<DataSeeder>(scope).SeedAsync()).IsSuccess);
        }

        Assert.False(DosyaVar("roles.json"));
        Assert.False(DosyaVar("users.json"));
        Assert.False(DosyaVar("userRoles.json"));
        Assert.False(DosyaVar("rolePermissions.json"));
    }
}
