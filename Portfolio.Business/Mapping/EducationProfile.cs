using AutoMapper;
using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Mapping;

public class EducationProfile : Profile
{
    public EducationProfile()
    {
        CreateMap<Education, EducationCreateDto>().ReverseMap();
        CreateMap<Education, EducationUpdateDto>().ReverseMap();
        CreateMap<Education, EducationListDto>().ReverseMap();
    }
}
