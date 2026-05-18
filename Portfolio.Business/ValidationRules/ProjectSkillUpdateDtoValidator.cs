using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class ProjectSkillUpdateDtoValidator : AbstractValidator<ProjectSkillUpdateDto>
{
    public ProjectSkillUpdateDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ProjectId).GreaterThan(0);
        RuleFor(x => x.SkillId).GreaterThan(0);
    }
}
