using AutoMapper;
using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Mapping;

public class ExperienceTypeProfile : Profile
{
    public ExperienceTypeProfile()
    {
        CreateMap<ExperienceType, ExperienceTypeCreateDto>().ReverseMap();
        CreateMap<ExperienceType, ExperienceTypeUpdateDto>().ReverseMap();
        CreateMap<ExperienceType, ExperienceTypeListDto>().ReverseMap();
    }
}
