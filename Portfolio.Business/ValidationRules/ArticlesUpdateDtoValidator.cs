using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class ArticlesUpdateDtoValidator : AbstractValidator<ArticleUpdateDto>
{
    public ArticlesUpdateDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}
