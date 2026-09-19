using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Portfolyo.Web.Infrastructure.Persistence.Json;

/// <summary>
/// Entity tipi başına tek bir JSON dosyasını yöneten depo.
/// Bir istek boyunca dosya bir kez okunur, değişiklikler bellekte biriktirilir ve
/// SaveChangesAsync ile tek seferde diske yazılır.
/// Scoped kaydedilir: bir isteğin kaydedilmemiş değişiklikleri başka isteği etkilemez.
/// </summary>
public sealed class JsonFileStore : IDisposable
{
    // Dosya bazlı kilit; scoped örnekler arasında paylaşılması için static.
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> FileLocks = new();

    private readonly JsonStoreOptions _options;
    private readonly Dictionary<Type, object> _collections = new();
    private readonly HashSet<Type> _dirtyTypes = new();

    public JsonFileStore(IOptions<JsonStoreOptions> options)
    {
        _options = options.Value;
        Directory.CreateDirectory(_options.RootPath);
    }

    /// <summary>Kaydedilmemiş değişiklik var mı?</summary>
    public bool HasChanges => _dirtyTypes.Count > 0;

    /// <summary>
    /// İlgili entity'nin listesini döner. İlk çağrıda dosyadan okunur, sonrasında bellekten.
    /// Dönen liste canlıdır; üzerinde yapılan değişiklikler MarkDirty ile işaretlenmelidir.
    /// </summary>
    public async Task<List<T>> GetCollectionAsync<T>(CancellationToken cancellationToken = default)
    {
        if (_collections.TryGetValue(typeof(T), out var cached))
        {
            return (List<T>)cached;
        }

        var items = await ReadFromDiskAsync<T>(cancellationToken);
        _collections[typeof(T)] = items;
        return items;
    }

    /// <summary>İlgili koleksiyonu "kaydedilecek" olarak işaretler.</summary>
    public void MarkDirty<T>() => _dirtyTypes.Add(typeof(T));

    /// <summary>
    /// İşaretli tüm koleksiyonları diske yazar ve yazılan dosya sayısını döner.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_dirtyTypes.Count == 0)
        {
            return 0;
        }

        var written = 0;

        foreach (var type in _dirtyTypes.ToList())
        {
            if (!_collections.TryGetValue(type, out var collection))
            {
                continue;
            }

            await WriteToDiskAsync(type, collection, cancellationToken);
            written++;
        }

        _dirtyTypes.Clear();
        return written;
    }

    /// <summary>Kaydedilmemiş değişiklikleri atar; koleksiyonlar tekrar diskten okunur.</summary>
    public void DiscardChanges()
    {
        foreach (var type in _dirtyTypes)
        {
            _collections.Remove(type);
        }

        _dirtyTypes.Clear();
    }

    private async Task<List<T>> ReadFromDiskAsync<T>(CancellationToken cancellationToken)
    {
        var path = GetFilePath(typeof(T));
        var gate = GetLock(path);

        await gate.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(path))
            {
                return new List<T>();
            }

            await using var stream = File.OpenRead(path);
            if (stream.Length == 0)
            {
                return new List<T>();
            }

            var items = await JsonSerializer.DeserializeAsync<List<T>>(
                stream, _options.SerializerOptions, cancellationToken);

            return items ?? new List<T>();
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task WriteToDiskAsync(Type type, object collection, CancellationToken cancellationToken)
    {
        var path = GetFilePath(type);
        var temporaryPath = path + ".tmp";
        var gate = GetLock(path);

        await gate.WaitAsync(cancellationToken);
        try
        {
            // Önce geçici dosyaya yazılır; yazma yarıda kalırsa asıl dosya bozulmaz.
            await using (var stream = File.Create(temporaryPath))
            {
                await JsonSerializer.SerializeAsync(
                    stream, collection, collection.GetType(), _options.SerializerOptions, cancellationToken);
            }

            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }

            gate.Release();
        }
    }

    private string GetFilePath(Type type) => Path.Combine(_options.RootPath, GetFileName(type));

    /// <summary>User -> users.json, RolePermission -> rolePermissions.json, Category -> categories.json</summary>
    private static string GetFileName(Type type)
    {
        var name = char.ToLowerInvariant(type.Name[0]) + type.Name[1..];

        var plural = name switch
        {
            _ when name.EndsWith('y') && !"aeiou".Contains(name[^2]) => name[..^1] + "ies",
            _ when name.EndsWith("s") || name.EndsWith("x") || name.EndsWith("ch") || name.EndsWith("sh") => name + "es",
            _ => name + "s"
        };

        return plural + ".json";
    }

    private static SemaphoreSlim GetLock(string path)
        => FileLocks.GetOrAdd(path.ToLowerInvariant(), _ => new SemaphoreSlim(1, 1));

    public void Dispose()
    {
        _collections.Clear();
        _dirtyTypes.Clear();
    }
}
