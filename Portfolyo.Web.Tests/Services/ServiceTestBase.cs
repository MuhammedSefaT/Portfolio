using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Portfolyo.Web.Application.Extensions;
using Portfolyo.Web.Infrastructure.Extensions;

namespace Portfolyo.Web.Tests.Services;

/// <summary>
/// Servis testleri için ortak kurulum: her test kendi geçici veri klasöründe çalışır,
/// her scope yeni bir istek gibi davranır.
/// </summary>
public abstract class ServiceTestBase : IDisposable
{
    private readonly ServiceProvider _provider;

    protected ServiceTestBase()
    {
        DataPath = Path.Combine(Path.GetTempPath(), "portfolyo-test-" + Guid.NewGuid().ToString("N"));

        var services = new ServiceCollection();
        services.AddLogging(); // AutoMapper ILoggerFactory bekliyor; web tarafında hazır geliyor.
        services.AddInfrastructure(DataPath);
        services.AddApplication();

        Yapilandir(services);

        _provider = services.BuildServiceProvider();
    }

    protected string DataPath { get; }

    /// <summary>Testin kendi kayıtlarını eklemesi veya ayar değiştirmesi için.</summary>
    protected virtual void Yapilandir(IServiceCollection services)
    {
    }

    protected IServiceScope YeniIstek() => _provider.CreateScope();

    protected static TService Servis<TService>(IServiceScope scope) where TService : notnull
        => scope.ServiceProvider.GetRequiredService<TService>();

    protected bool DosyaVar(string fileName) => File.Exists(Path.Combine(DataPath, fileName));

    protected Task<string> DosyaOkuAsync(string fileName)
        => File.ReadAllTextAsync(Path.Combine(DataPath, fileName));

    public void Dispose()
    {
        _provider.Dispose();

        if (Directory.Exists(DataPath))
        {
            Directory.Delete(DataPath, recursive: true);
        }

        GC.SuppressFinalize(this);
    }
}
