using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class EducationCreateDtoValidator : AbstractValidator<EducationCreateDto>
{
    public EducationCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Institution).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Description).NotEmpty();
    }
}
