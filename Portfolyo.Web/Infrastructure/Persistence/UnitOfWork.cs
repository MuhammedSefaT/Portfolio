using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Core.Entities;
using Portfolyo.Web.Infrastructure.Persistence.Json;

namespace Portfolyo.Web.Infrastructure.Persistence;

/// <summary>
/// JSON dosya deposu üzerinde çalışan UnitOfWork.
/// Repository'ler aynı JsonFileStore örneğini paylaştığı için tek SaveChangesAsync
/// çağrısı bütün değişen dosyaları kaydeder.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly JsonFileStore _store;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(JsonFileStore store, IServiceProvider serviceProvider)
    {
        _store = store;
        _serviceProvider = serviceProvider;
    }

    public bool HasChanges => _store.HasChanges;

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        if (_repositories.TryGetValue(typeof(T), out var existing))
        {
            return (IRepository<T>)existing;
        }

        // Entity'ye özel bir repository kayıtlıysa o kullanılır, yoksa generic olanı üretilir.
        var repository = (IRepository<T>?)_serviceProvider.GetService(typeof(IRepository<T>))
                         ?? new Repositories.Repository<T>(_store);

        _repositories[typeof(T)] = repository;
        return repository;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _store.SaveChangesAsync(cancellationToken);

    public void DiscardChanges()
    {
        _store.DiscardChanges();

        // Atılan koleksiyonlar tekrar diskten okunacağı için repository önbelleği de boşaltılır.
        _repositories.Clear();
    }
}
