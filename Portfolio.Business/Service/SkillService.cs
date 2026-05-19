using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class SkillService : GenericService<Skill, SkillListDto, SkillCreateDto, SkillUpdateDto>, ISkillService
{
    public SkillService(IUow uow, IGenericRepository<Skill> repository, IMapper mapper, IValidator<SkillCreateDto> createValidator, IValidator<SkillUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
