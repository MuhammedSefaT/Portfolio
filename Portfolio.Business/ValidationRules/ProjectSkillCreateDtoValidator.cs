using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class ProjectSkillCreateDtoValidator : AbstractValidator<ProjectSkillCreateDto>
{
    public ProjectSkillCreateDtoValidator()
    {
        RuleFor(x => x.ProjectId).GreaterThan(0);
        RuleFor(x => x.SkillId).GreaterThan(0);
    }
}
