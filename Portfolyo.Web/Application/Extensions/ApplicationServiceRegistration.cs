using System.Reflection;
using FluentValidation;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Services;

namespace Portfolyo.Web.Application.Extensions;

public static class ApplicationServiceRegistration
{
    /// <summary>
    /// AutoMapper profillerini, FluentValidation doğrulayıcılarını ve generic servisi kaydeder.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(configuration => configuration.AddMaps(assembly));
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        // Açık generic kayıt: her entity için ayrı satır yazmaya gerek kalmaz.
        services.AddScoped(typeof(IService<,,,>), typeof(Service<,,,>));

        return services;
    }
}
