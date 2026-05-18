using AutoMapper;
using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Mapping;

public class SkillProfile : Profile
{
    public SkillProfile()
    {
        CreateMap<Skill, SkillCreateDto>().ReverseMap();
        CreateMap<Skill, SkillUpdateDto>().ReverseMap();
        CreateMap<Skill, SkillListDto>().ReverseMap();
    }
}
