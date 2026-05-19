using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Interface;
using Portfolio.Common.Result;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class GenericService<TEntity, TListDto, TCreateDto, TUpdateDto> : IGenericService<TEntity, TListDto, TCreateDto, TUpdateDto>
    where TEntity : BaseEntity
    where TListDto : IListDto
    where TCreateDto : ICreateDto
    where TUpdateDto : IUpdateDto
{
    protected readonly IUow _uow;
    protected readonly IGenericRepository<TEntity> _repository;
    protected readonly IMapper _mapper;
    protected readonly IValidator<TCreateDto> _createValidator;
    protected readonly IValidator<TUpdateDto> _updateValidator;

    public GenericService(IUow uow, IGenericRepository<TEntity> repository, IMapper mapper, IValidator<TCreateDto> createValidator, IValidator<TUpdateDto> updateValidator)
    {
        _uow = uow;
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<TListDto>> CreateAsync(TCreateDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResult = await _createValidator.ValidateAsync(createDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<TListDto>.Fail(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var entity = _mapper.Map<TEntity>(createDto);
            await _repository.CreateAsync(entity);
            await _uow.SaveChangesAsync();

            return Result<TListDto>.Ok(_mapper.Map<TListDto>(entity));
        }
        catch (Exception ex)
        {
            return Result<TListDto>.Fail(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _repository.GetAsync(id);
            if (entity is null)
            {
                return Result.Fail("Kayıt bulunamadı!");
            }

            _repository.Delete(entity);
            await _uow.SaveChangesAsync();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result<IReadOnlyList<TListDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var list = await _repository.GetAllAsync();
            if (list is null)
            {
                return Result<IReadOnlyList<TListDto>>.Fail("Listelenecek kayıt bulunamadı!");
            }
            return Result<IReadOnlyList<TListDto>>.Ok(_mapper.Map<IReadOnlyList<TListDto>>(list));
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<TListDto>>.Fail(ex.Message);
        }
    }

    public async Task<Result<TListDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _repository.GetAsync(id);
            if (entity is null)
            {
                return Result<TListDto>.Fail("Kayıt bulunamadı!");
            }

            return Result<TListDto>.Ok(_mapper.Map<TListDto>(entity));
        }
        catch (Exception ex)
        {

            return Result<TListDto>.Fail(ex.Message);
        }
    }

    public async Task<Result> UpdateAsync(TUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResult = await _updateValidator.ValidateAsync(updateDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var unchangedEntity = await _repository.GetAsync(updateDto.Id);
            if (unchangedEntity is null)
            {
                return Result.Fail("Kayıt bulunamadı!");
            }

            var entity = _mapper.Map<TEntity>(updateDto);
            _repository.Update(entity, unchangedEntity);
            await _uow.SaveChangesAsync();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
