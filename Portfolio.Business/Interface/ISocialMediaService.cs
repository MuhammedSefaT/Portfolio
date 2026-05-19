using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface ISocialMediaService : IGenericService<SocialMedia, SocialMediaListDto, SocialMediaCreateDto, SocialMediaUpdateDto>
{
}
