using Portfolio.Common.Interface;
using Portfolio.Common.Enum;

namespace Portfolio.Common.Dtos;

public class ContactMessageListDto : IListDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ContactMessageType ContactMessageType { get; set; } = ContactMessageType.Waiting;
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
