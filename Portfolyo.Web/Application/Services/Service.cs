using AutoMapper;
using FluentValidation;
using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common;
using Portfolyo.Web.Application.Common.Exceptions;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Services;

/// <summary>
/// IService'in ortak implementasyonu. Entity'ye özel kural gerektiğinde
/// bu sınıf miras alınıp ilgili metot override edilir.
/// </summary>
public class Service<TEntity, TDto, TCreateDto, TUpdateDto> : IService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : BaseEntity
    where TUpdateDto : IHasId
{
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly IMapper Mapper;

    // Doğrulayıcı kayıtlı değilse koleksiyon boş gelir; servis yine çalışır.
    private readonly IEnumerable<IValidator<TCreateDto>> _createValidators;
    private readonly IEnumerable<IValidator<TUpdateDto>> _updateValidators;

    public Service(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEnumerable<IValidator<TCreateDto>> createValidators,
        IEnumerable<IValidator<TUpdateDto>> updateValidators)
    {
        UnitOfWork = unitOfWork;
        Mapper = mapper;
        _createValidators = createValidators;
        _updateValidators = updateValidators;
    }

    protected IRepository<TEntity> Repository => UnitOfWork.Repository<TEntity>();

    public virtual async Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetAllAsync(cancellationToken);
        return Mapper.Map<List<TDto>>(entities);
    }

    public virtual async Task<IReadOnlyList<TDto>> GetWhereAsync(
        Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetWhereAsync(predicate, cancellationToken);
        return Mapper.Map<List<TDto>>(entities);
    }

    public virtual async Task<TDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : Mapper.Map<TDto>(entity);
    }

    public virtual Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => Repository.AnyAsync(x => x.Id == id, cancellationToken);

    public virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
        => Repository.CountAsync(cancellationToken: cancellationToken);

    public virtual async Task<TDto> CreateAsync(TCreateDto dto, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidators, dto, cancellationToken);

        var entity = Mapper.Map<TEntity>(dto);

        await Repository.AddAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto> UpdateAsync(TUpdateDto dto, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidators, dto, cancellationToken);

        var entity = await Repository.GetByIdAsync(dto.Id, cancellationToken)
                     ?? throw NotFoundException.For<TEntity>(dto.Id);

        // DTO mevcut kaydın üzerine yazılır; DTO'da olmayan alanlar (ör. PasswordHash) korunur.
        Mapper.Map(dto, entity);

        await Repository.UpdateAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<TDto>(entity);
    }

    public virtual async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await Repository.DeleteAsync(id, cancellationToken))
        {
            return false;
        }

        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static async Task ValidateAsync<TInput>(
        IEnumerable<IValidator<TInput>> validators, TInput input, CancellationToken cancellationToken)
    {
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(input, cancellationToken);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}
