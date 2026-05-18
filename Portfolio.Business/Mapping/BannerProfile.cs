using AutoMapper;
using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Mapping;

public class BannerProfile : Profile
{
    public BannerProfile()
    {
        CreateMap<Banner, BannerCreateDto>().ReverseMap();
        CreateMap<Banner, BannerUpdateDto>().ReverseMap();
        CreateMap<Banner, BannerListDto>().ReverseMap();
    }
}
