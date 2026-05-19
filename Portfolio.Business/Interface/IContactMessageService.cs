using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface IContactMessageService : IGenericService<ContactMessage, ContactMessageListDto, ContactMessageCreateDto, ContactMessageUpdateDto>
{
}
