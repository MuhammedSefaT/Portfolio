using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class BannerUpdateDtoValidator : AbstractValidator<BannerUpdateDto>
{
    public BannerUpdateDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ImageUrl).NotEmpty();
    }
}
