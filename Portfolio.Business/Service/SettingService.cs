using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class SettingService : GenericService<Setting, SettingListDto, SettingCreateDto, SettingUpdateDto>, ISettingService
{
    public SettingService(IUow uow, IGenericRepository<Setting> repository, IMapper mapper, IValidator<SettingCreateDto> createValidator, IValidator<SettingUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
