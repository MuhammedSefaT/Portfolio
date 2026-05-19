using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class ProjectSkillService : GenericService<ProjectSkill, ProjectSkillListDto, ProjectSkillCreateDto, ProjectSkillUpdateDto>, IProjectSkillService
{
    public ProjectSkillService(IUow uow, IGenericRepository<ProjectSkill> repository, IMapper mapper, IValidator<ProjectSkillCreateDto> createValidator, IValidator<ProjectSkillUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
