using AutoMapper;
using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Mapping;

public class ExperienceProfile : Profile
{
    public ExperienceProfile()
    {
        CreateMap<Experience, ExperienceCreateDto>().ReverseMap();
        CreateMap<Experience, ExperienceUpdateDto>().ReverseMap();
        CreateMap<Experience, ExperienceListDto>().ReverseMap();
    }
}
