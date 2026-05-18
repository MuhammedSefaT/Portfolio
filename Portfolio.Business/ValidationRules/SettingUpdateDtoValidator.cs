using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class SettingUpdateDtoValidator : AbstractValidator<SettingUpdateDto>
{
    public SettingUpdateDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Key).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Value).MaximumLength(1000);
        RuleFor(x => x.InputType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Options).MaximumLength(1000);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
