using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class ProjectUpdateDtoValidator : AbstractValidator<ProjectUpdateDto>
{
    public ProjectUpdateDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
        RuleFor(x => x.IconClass).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.GitHubUrl).MaximumLength(500);
        RuleFor(x => x.WebSiteUrl).MaximumLength(500);
    }
}
