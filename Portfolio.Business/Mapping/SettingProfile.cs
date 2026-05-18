using AutoMapper;
using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Mapping;

public class SettingProfile : Profile
{
    public SettingProfile()
    {
        CreateMap<Setting, SettingCreateDto>().ReverseMap();
        CreateMap<Setting, SettingUpdateDto>().ReverseMap();
        CreateMap<Setting, SettingListDto>().ReverseMap();
    }
}
