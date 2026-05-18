using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class ExperienceCreateDtoValidator : AbstractValidator<ExperienceCreateDto>
{
    public ExperienceCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.BusinessName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.ExperienceId).GreaterThan(0);
    }
}
