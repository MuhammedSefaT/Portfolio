using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface IEducationService : IGenericService<Education, EducationListDto, EducationCreateDto, EducationUpdateDto>
{
}
