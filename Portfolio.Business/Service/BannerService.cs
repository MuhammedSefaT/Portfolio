using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class BannerService : GenericService<Banner, BannerListDto, BannerCreateDto, BannerUpdateDto>, IBannerService
{
    public BannerService(IUow uow, IGenericRepository<Banner> repository, IMapper mapper, IValidator<BannerCreateDto> createValidator, IValidator<BannerUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
