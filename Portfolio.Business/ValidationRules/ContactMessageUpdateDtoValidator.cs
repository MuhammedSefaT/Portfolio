using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class ContactMessageUpdateDtoValidator : AbstractValidator<ContactMessageUpdateDto>
{
    public ContactMessageUpdateDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(150).EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Description).NotEmpty();
    }
}
