using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class ContactMessageService : GenericService<ContactMessage, ContactMessageListDto, ContactMessageCreateDto, ContactMessageUpdateDto>, IContactMessageService
{
    public ContactMessageService(IUow uow, IGenericRepository<ContactMessage> repository, IMapper mapper, IValidator<ContactMessageCreateDto> createValidator, IValidator<ContactMessageUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
