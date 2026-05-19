using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class CategoryService : GenericService<Category, CategoryListDto, CategoryCreateDto, CategoryUpdateDto>, ICategoryService
{
    public CategoryService(IUow uow, IGenericRepository<Category> repository, IMapper mapper, IValidator<CategoryCreateDto> createValidator, IValidator<CategoryUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
