using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class ProjectService : GenericService<Project, ProjectListDto, ProjectCreateDto, ProjectUpdateDto>, IProjectService
{
    public ProjectService(IUow uow, IGenericRepository<Project> repository, IMapper mapper, IValidator<ProjectCreateDto> createValidator, IValidator<ProjectUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
