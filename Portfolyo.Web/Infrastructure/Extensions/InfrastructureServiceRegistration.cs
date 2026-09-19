using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Infrastructure.Persistence;
using Portfolyo.Web.Infrastructure.Persistence.Json;
using Portfolyo.Web.Infrastructure.Persistence.Repositories;
using Portfolyo.Web.Infrastructure.Services;
using IApplicationPasswordHasher = Portfolyo.Web.Application.Abstractions.Services.IPasswordHasher;

namespace Portfolyo.Web.Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    /// <summary>
    /// JSON dosya deposu, repository'ler ve UnitOfWork kayıtlarını yapar.
    /// Veri klasörü ContentRootPath altında çözümlenir.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IHostEnvironment environment,
        string dataFolder = "App_Data/json")
        => services.AddInfrastructure(Path.Combine(environment.ContentRootPath, dataFolder));

    /// <summary>
    /// Veri klasörünün mutlak yolu verilerek kayıt yapar.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dataRootPath)
    {
        services.Configure<JsonStoreOptions>(options => options.RootPath = dataRootPath);

        // Scoped: her isteğin kendi önbelleği ve kaydedilmemiş değişiklikleri olur.
        services.AddScoped<JsonFileStore>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IApplicationPasswordHasher, PasswordHasher>();

        return services;
    }
}
