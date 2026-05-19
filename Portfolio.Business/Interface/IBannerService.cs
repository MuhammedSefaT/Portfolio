using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface IBannerService : IGenericService<Banner, BannerListDto, BannerCreateDto, BannerUpdateDto>
{
}
