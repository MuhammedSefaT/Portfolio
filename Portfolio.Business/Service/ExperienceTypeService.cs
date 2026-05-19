using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class ExperienceTypeService : GenericService<ExperienceType, ExperienceTypeListDto, ExperienceTypeCreateDto, ExperienceTypeUpdateDto>, IExperienceTypeService
{
    public ExperienceTypeService(IUow uow, IGenericRepository<ExperienceType> repository, IMapper mapper, IValidator<ExperienceTypeCreateDto> createValidator, IValidator<ExperienceTypeUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
