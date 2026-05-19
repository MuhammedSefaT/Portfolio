using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class ArticleService : GenericService<Article, ArticleListDto, ArticleCreateDto, ArticleUpdateDto>, IArticleService
{
    public ArticleService(IUow uow, IGenericRepository<Article> repository, IMapper mapper, IValidator<ArticleCreateDto> createValidator, IValidator<ArticleUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
