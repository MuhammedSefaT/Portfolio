using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface ICategoryService : IGenericService<Category, CategoryListDto, CategoryCreateDto, CategoryUpdateDto>
{
}
