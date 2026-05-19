using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface ISettingService : IGenericService<Setting, SettingListDto, SettingCreateDto, SettingUpdateDto>
{
}
