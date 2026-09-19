using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Abstractions.Repositories;

/// <summary>
/// Repository'ler üzerinden yapılan değişiklikleri tek noktada diske yazar.
/// Bir işlemde birden çok dosya değişse bile hepsi aynı çağrıda kaydedilir.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>İlgili entity için repository'yi döner; aynı istek içinde aynı örnek kullanılır.</summary>
    IRepository<T> Repository<T>() where T : BaseEntity;

    /// <summary>Kaydedilmemiş değişiklik var mı?</summary>
    bool HasChanges { get; }

    /// <summary>Değişen dosyaları diske yazar ve yazılan dosya sayısını döner.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Kaydedilmemiş değişiklikleri atar.</summary>
    void DiscardChanges();
}
