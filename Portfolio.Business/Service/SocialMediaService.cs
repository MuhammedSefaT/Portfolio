using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class SocialMediaService : GenericService<SocialMedia, SocialMediaListDto, SocialMediaCreateDto, SocialMediaUpdateDto>, ISocialMediaService
{
    public SocialMediaService(IUow uow, IGenericRepository<SocialMedia> repository, IMapper mapper, IValidator<SocialMediaCreateDto> createValidator, IValidator<SocialMediaUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
