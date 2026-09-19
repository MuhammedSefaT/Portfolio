using AutoMapper;
using FluentValidation;
using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Services;

/// <summary>
/// IService'in ortak implementasyonu. Entity'ye özel servisler bu sınıfı miras alır;
/// ek kurallar için OnCreating / OnUpdating / OnDeleting kancalarını kullanır.
/// </summary>
public abstract class Service<TEntity, TDto, TCreateDto, TUpdateDto> : IService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : BaseEntity
    where TUpdateDto : IHasId
{
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly IMapper Mapper;

    // Doğrulayıcı kayıtlı değilse koleksiyon boş gelir; servis yine çalışır.
    private readonly IEnumerable<IValidator<TCreateDto>> _createValidators;
    private readonly IEnumerable<IValidator<TUpdateDto>> _updateValidators;

    protected Service(
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

    /// <summary>Kayıt bulunamadığında dönecek mesaj; alt sınıflar özelleştirebilir.</summary>
    protected virtual string NotFoundMessage => "Kayıt bulunamadı.";

    public virtual async Task<Result<IReadOnlyList<TDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyList<TDto>>.Success(Mapper.Map<List<TDto>>(entities));
    }

    public virtual async Task<Result<IReadOnlyList<TDto>>> GetWhereAsync(
        Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetWhereAsync(predicate, cancellationToken);
        return Result<IReadOnlyList<TDto>>.Success(Mapper.Map<List<TDto>>(entities));
    }

    public virtual async Task<Result<TDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken);

        return entity is null
            ? Result<TDto>.NotFound(NotFoundMessage)
            : Result<TDto>.Success(Mapper.Map<TDto>(entity));
    }

    public virtual async Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => Result<bool>.Success(await Repository.AnyAsync(x => x.Id == id, cancellationToken));

    public virtual async Task<Result<int>> CountAsync(CancellationToken cancellationToken = default)
        => Result<int>.Success(await Repository.CountAsync(cancellationToken: cancellationToken));

    public virtual async Task<Result<TDto>> CreateAsync(TCreateDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(_createValidators, dto, cancellationToken);

        if (validation.IsFailure)
        {
            return validation.CarryFailure<TDto>();
        }

        var entity = Mapper.Map<TEntity>(dto);

        // Alt sınıfın ek kuralları (ör. parola hash'leme, tekillik denetimi).
        var hook = await OnCreatingAsync(dto, entity, cancellationToken);

        if (hook.IsFailure)
        {
            return hook.CarryFailure<TDto>();
        }

        await Repository.AddAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TDto>.Success(Mapper.Map<TDto>(entity));
    }

    public virtual async Task<Result<TDto>> UpdateAsync(TUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(_updateValidators, dto, cancellationToken);

        if (validation.IsFailure)
        {
            return validation.CarryFailure<TDto>();
        }

        var entity = await Repository.GetByIdAsync(dto.Id, cancellationToken);

        if (entity is null)
        {
            return Result<TDto>.NotFound(NotFoundMessage);
        }

        // DTO mevcut kaydın üzerine yazılır; DTO'da olmayan alanlar (ör. PasswordHash) korunur.
        Mapper.Map(dto, entity);

        var hook = await OnUpdatingAsync(dto, entity, cancellationToken);

        if (hook.IsFailure)
        {
            // Kural ihlali varsa bellekteki değişiklik diske yazılmadan atılır.
            UnitOfWork.DiscardChanges();
            return hook.CarryFailure<TDto>();
        }

        await Repository.UpdateAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TDto>.Success(Mapper.Map<TDto>(entity));
    }

    public virtual async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return Result.NotFound(NotFoundMessage);
        }

        var hook = await OnDeletingAsync(entity, cancellationToken);

        if (hook.IsFailure)
        {
            return hook;
        }

        await Repository.DeleteAsync(id, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    /// <summary>Kayıt eklenmeden önce çalışır. Başarısız dönerse kayıt oluşturulmaz.</summary>
    protected virtual Task<Result> OnCreatingAsync(TCreateDto dto, TEntity entity, CancellationToken cancellationToken)
        => Task.FromResult(Result.Success());

    /// <summary>Kayıt güncellenmeden önce çalışır. Başarısız dönerse güncelleme yapılmaz.</summary>
    protected virtual Task<Result> OnUpdatingAsync(TUpdateDto dto, TEntity entity, CancellationToken cancellationToken)
        => Task.FromResult(Result.Success());

    /// <summary>Kayıt silinmeden önce çalışır. Başarısız dönerse silme yapılmaz.</summary>
    protected virtual Task<Result> OnDeletingAsync(TEntity entity, CancellationToken cancellationToken)
        => Task.FromResult(Result.Success());

    private static async Task<Result<bool>> ValidateAsync<TInput>(
        IEnumerable<IValidator<TInput>> validators, TInput input, CancellationToken cancellationToken)
    {
        var errors = new List<ResultError>();

        foreach (var validator in validators)
        {
            var validation = await validator.ValidateAsync(input, cancellationToken);

            if (!validation.IsValid)
            {
                errors.AddRange(validation.Errors.Select(x => new ResultError(x.PropertyName, x.ErrorMessage)));
            }
        }

        return errors.Count == 0
            ? Result<bool>.Success(true)
            : Result<bool>.Invalid(errors);
    }
}
