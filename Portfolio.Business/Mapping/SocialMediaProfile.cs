using AutoMapper;
using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Mapping;

public class SocialMediaProfile : Profile
{
    public SocialMediaProfile()
    {
        CreateMap<SocialMedia, SocialMediaCreateDto>().ReverseMap();
        CreateMap<SocialMedia, SocialMediaUpdateDto>().ReverseMap();
        CreateMap<SocialMedia, SocialMediaListDto>().ReverseMap();
    }
}
