using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface IProjectService : IGenericService<Project, ProjectListDto, ProjectCreateDto, ProjectUpdateDto>
{
}
