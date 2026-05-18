using AutoMapper;
using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Mapping;

public class ContactMessageProfile : Profile
{
    public ContactMessageProfile()
    {
        CreateMap<ContactMessage, ContactMessageCreateDto>().ReverseMap();
        CreateMap<ContactMessage, ContactMessageUpdateDto>().ReverseMap();
        CreateMap<ContactMessage, ContactMessageListDto>().ReverseMap();
    }
}
