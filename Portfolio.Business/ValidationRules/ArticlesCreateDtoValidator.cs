using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class ArticlesCreateDtoValidator : AbstractValidator<ArticleCreateDto>
{
    public ArticlesCreateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}
