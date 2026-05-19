using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface IExperienceService : IGenericService<Experience, ExperienceListDto, ExperienceCreateDto, ExperienceUpdateDto>
{
}
