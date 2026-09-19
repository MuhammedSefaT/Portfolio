using Portfolyo.Web.Application.Common;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Abstractions.Services;

/// <summary>
/// Entity başına tekrar yazmamak için ortak CRUD servisi.
/// Doğrulama, eşleme ve kaydetme bu katmanda yapılır.
/// </summary>
/// <typeparam name="TEntity">Veri kaynağındaki entity.</typeparam>
/// <typeparam name="TDto">Dışarıya dönen tip.</typeparam>
/// <typeparam name="TCreateDto">Oluşturma girdisi.</typeparam>
/// <typeparam name="TUpdateDto">Güncelleme girdisi.</typeparam>
public interface IService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : BaseEntity
    where TUpdateDto : IHasId
{
    Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TDto>> GetWhereAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default);

    Task<TDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>Doğrulama başarısızsa FluentValidation.ValidationException fırlatır.</summary>
    Task<TDto> CreateAsync(TCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Kayıt yoksa NotFoundException fırlatır.</summary>
    Task<TDto> UpdateAsync(TUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Kayıt yoksa false döner.</summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
