using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class EducationService : GenericService<Education, EducationListDto, EducationCreateDto, EducationUpdateDto>, IEducationService
{
    public EducationService(IUow uow, IGenericRepository<Education> repository, IMapper mapper, IValidator<EducationCreateDto> createValidator, IValidator<EducationUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
