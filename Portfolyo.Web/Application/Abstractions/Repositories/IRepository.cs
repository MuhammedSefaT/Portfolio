using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Abstractions.Repositories;

/// <summary>
/// Tüm entity'ler için ortak veri erişim sözleşmesi.
/// Yazma metotları değişikliği yalnızca bellekte işaretler; diske yazma
/// IUnitOfWork.SaveChangesAsync çağrısıyla yapılır.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Filtreler bellekteki liste üzerinde çalıştığı için Expression değil Func alınır.
    /// </summary>
    Task<IReadOnlyList<T>> GetWhereAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<T?> GetFirstOrDefaultAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default);

    Task<int> CountAsync(Func<T, bool>? predicate = null, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>Kayıt bulunamazsa false döner.</summary>
    Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Kayıt bulunamazsa false döner.</summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> DeleteWhereAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default);
}
