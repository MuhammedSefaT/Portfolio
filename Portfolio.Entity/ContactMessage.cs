using Portfolio.Common.Enum;

namespace Portfolio.Entity;

public class ContactMessage : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ContactMessageType ContactMessageType { get; set; } = ContactMessageType.Waiting;
}
