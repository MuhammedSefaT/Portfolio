using Portfolyo.Web.Infrastructure.Persistence.Seed;

namespace Portfolyo.Web.Infrastructure.Extensions;

public static class SeedExtensions
{
    /// <summary>
    /// Uygulama açılışında Admin rolünü, yetkileri ve yönetici kullanıcısını hazırlar.
    /// Servisler scoped olduğu için kendi scope'unu açar.
    /// </summary>
    public static async Task UseDataSeedAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();

        await seeder.SeedAsync(cancellationToken);
    }
}
