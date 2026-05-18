using AutoMapper;
using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Mapping;

public class ArticlesProfile : Profile
{
    public ArticlesProfile()
    {
        CreateMap<Articles, ArticlesCreateDto>().ReverseMap();
        CreateMap<Articles, ArticlesUpdateDto>().ReverseMap();
        CreateMap<Articles, ArticlesListDto>().ReverseMap();
    }
}
