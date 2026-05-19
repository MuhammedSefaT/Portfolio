using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface ISkillService : IGenericService<Skill, SkillListDto, SkillCreateDto, SkillUpdateDto>
{
}
