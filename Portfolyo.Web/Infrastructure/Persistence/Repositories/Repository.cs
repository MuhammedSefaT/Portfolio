using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Application.Common.Exceptions;
using Portfolyo.Web.Core.Entities;
using Portfolyo.Web.Infrastructure.Persistence.Json;

namespace Portfolyo.Web.Infrastructure.Persistence.Repositories;

/// <summary>
/// IRepository'nin JSON dosyası üzerinde çalışan ortak implementasyonu.
/// Yazma metotları diske dokunmaz; değişikliği işaretler, kaydetmeyi UnitOfWork yapar.
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly JsonFileStore Store;

    public Repository(JsonFileStore store) => Store = store;

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await Store.GetCollectionAsync<T>(cancellationToken);
        return items.ToList();
    }

    public async Task<IReadOnlyList<T>> GetWhereAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default)
    {
        var items = await Store.GetCollectionAsync<T>(cancellationToken);
        return items.Where(predicate).ToList();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var items = await Store.GetCollectionAsync<T>(cancellationToken);
        return items.FirstOrDefault(x => x.Id == id);
    }

    public async Task<T?> GetFirstOrDefaultAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default)
    {
        var items = await Store.GetCollectionAsync<T>(cancellationToken);
        return items.FirstOrDefault(predicate);
    }

    public async Task<bool> AnyAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default)
    {
        var items = await Store.GetCollectionAsync<T>(cancellationToken);
        return items.Any(predicate);
    }

    public async Task<int> CountAsync(Func<T, bool>? predicate = null, CancellationToken cancellationToken = default)
    {
        var items = await Store.GetCollectionAsync<T>(cancellationToken);
        return predicate is null ? items.Count : items.Count(predicate);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var items = await Store.GetCollectionAsync<T>(cancellationToken);

        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        entity.CreatedAt = DateTime.UtcNow;

        items.Add(entity);
        Store.MarkDirty<T>();
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity, cancellationToken);
        }
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var items = await Store.GetCollectionAsync<T>(cancellationToken);
        var index = items.FindIndex(x => x.Id == entity.Id);

        if (index < 0)
        {
            throw NotFoundException.For<T>(entity.Id);
        }

        entity.CreatedAt = items[index].CreatedAt;
        entity.UpdatedAt = DateTime.UtcNow;

        items[index] = entity;
        Store.MarkDirty<T>();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var items = await Store.GetCollectionAsync<T>(cancellationToken);
        var index = items.FindIndex(x => x.Id == id);

        if (index < 0)
        {
            return false;
        }

        items.RemoveAt(index);
        Store.MarkDirty<T>();
        return true;
    }

    public async Task<int> DeleteWhereAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default)
    {
        var items = await Store.GetCollectionAsync<T>(cancellationToken);
        var removed = items.RemoveAll(new Predicate<T>(predicate));

        if (removed > 0)
        {
            Store.MarkDirty<T>();
        }

        return removed;
    }
}
