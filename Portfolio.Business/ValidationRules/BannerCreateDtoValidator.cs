using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class BannerCreateDtoValidator : AbstractValidator<BannerCreateDto>
{
    public BannerCreateDtoValidator()
    {
        RuleFor(x => x.ImageUrl).NotEmpty();
    }
}
