using Portfolyo.Web.Application.Common;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Abstractions.Services;

/// <summary>
/// Entity başına tekrar yazmamak için ortak CRUD sözleşmesi.
/// Entity'ye özel servisler bu arayüzü genişletir; controller'lar doğrudan bunu değil
/// ilgili entity'nin servisini (ör. IRoleService) kullanır.
/// Veri taşıyan işlemler Result&lt;T&gt;, taşımayanlar düz Result döner.
/// </summary>
/// <typeparam name="TEntity">Veri kaynağındaki entity.</typeparam>
/// <typeparam name="TDto">Dışarıya dönen tip.</typeparam>
/// <typeparam name="TCreateDto">Oluşturma girdisi.</typeparam>
/// <typeparam name="TUpdateDto">Güncelleme girdisi.</typeparam>
public interface IService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : BaseEntity
    where TUpdateDto : IHasId
{
    Task<Result<IReadOnlyList<TDto>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<TDto>>> GetWhereAsync(
        Func<TEntity, bool> predicate, CancellationToken cancellationToken = default);

    /// <summary>Kayıt yoksa Status = NotFound döner.</summary>
    Task<Result<TDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<int>> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>Doğrulama başarısızsa Status = ValidationError ve Errors dolu döner.</summary>
    Task<Result<TDto>> CreateAsync(TCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Kayıt yoksa Status = NotFound döner.</summary>
    Task<Result<TDto>> UpdateAsync(TUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Veri taşımadığı için düz Result döner.</summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
