using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface IExperienceTypeService : IGenericService<ExperienceType, ExperienceTypeListDto, ExperienceTypeCreateDto, ExperienceTypeUpdateDto>
{
}
