using Microsoft.Extensions.DependencyInjection;
using Portfolio.Business.Interface;
using Portfolio.DataAccess.Intrerface;
using Portfolio.DataAccess.Repository;
using Portfolio.DataAccess.UnitOfWork;

namespace Portfolio.Business.Extension;

public static class DependencyExtension
{
    public static void AddBusiness(this IServiceCollection services)
    {
        services.AddScoped<IUow, Uow>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(IGenericService<,,,>), typeof(IGenericService<,,,>));
    }
}
