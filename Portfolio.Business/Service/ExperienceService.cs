using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class ExperienceService : GenericService<Experience, ExperienceListDto, ExperienceCreateDto, ExperienceUpdateDto>, IExperienceService
{
    public ExperienceService(IUow uow, IGenericRepository<Experience> repository, IMapper mapper, IValidator<ExperienceCreateDto> createValidator, IValidator<ExperienceUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
