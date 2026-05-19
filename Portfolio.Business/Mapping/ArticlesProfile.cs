using AutoMapper;
using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Mapping;

public class ArticlesProfile : Profile
{
    public ArticlesProfile()
    {
        CreateMap<Article, ArticleCreateDto>().ReverseMap();
        CreateMap<Article, ArticleUpdateDto>().ReverseMap();
        CreateMap<Article, ArticleListDto>().ReverseMap();
    }
}
