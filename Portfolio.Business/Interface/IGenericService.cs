using Portfolio.Common.Interface;
using Portfolio.Common.Result;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface IGenericService<TEntity, TListDto, TCreateDto, TUpdateDto>
    where TEntity : BaseEntity
    where TListDto : IListDto
    where TCreateDto : ICreateDto
    where TUpdateDto : IUpdateDto
{
    Task<Result<IReadOnlyList<TListDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<TListDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<TListDto>> CreateAsync(TCreateDto createDto, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(TUpdateDto updateDto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
