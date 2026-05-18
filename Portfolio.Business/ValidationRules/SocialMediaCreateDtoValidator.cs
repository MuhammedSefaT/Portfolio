using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class SocialMediaCreateDtoValidator : AbstractValidator<SocialMediaCreateDto>
{
    public SocialMediaCreateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.IconClass).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ContactUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
