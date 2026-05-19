using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface IArticleService : IGenericService<Article, ArticleListDto, ArticleCreateDto, ArticleUpdateDto>
{
}
