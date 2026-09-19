using System.Reflection;
using FluentValidation;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Services;

namespace Portfolyo.Web.Application.Extensions;

public static class ApplicationServiceRegistration
{
    /// <summary>
    /// AutoMapper profillerini, FluentValidation doğrulayıcılarını ve
    /// entity bazlı servisleri kaydeder.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(configuration => configuration.AddMaps(assembly));
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        // Controller'lar generic servisi değil, entity'nin kendi servisini kullanır.
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();

        return services;
    }
}
