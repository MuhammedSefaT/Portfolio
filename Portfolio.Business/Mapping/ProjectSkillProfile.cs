using AutoMapper;
using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Mapping;

public class ProjectSkillProfile : Profile
{
    public ProjectSkillProfile()
    {
        CreateMap<ProjectSkill, ProjectSkillCreateDto>().ReverseMap();
        CreateMap<ProjectSkill, ProjectSkillUpdateDto>().ReverseMap();
        CreateMap<ProjectSkill, ProjectSkillListDto>().ReverseMap();
    }
}
