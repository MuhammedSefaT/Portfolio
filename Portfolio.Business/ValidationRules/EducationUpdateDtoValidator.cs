using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class EducationUpdateDtoValidator : AbstractValidator<EducationUpdateDto>
{
    public EducationUpdateDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Institution).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Description).NotEmpty();
    }
}
