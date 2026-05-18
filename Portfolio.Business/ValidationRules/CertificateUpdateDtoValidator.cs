using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class CertificateUpdateDtoValidator : AbstractValidator<CertificateUpdateDto>
{
    public CertificateUpdateDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Issuer).NotEmpty().MaximumLength(250);
        RuleFor(x => x.VerificationUrl).MaximumLength(500);
        RuleFor(x => x.ImageUrl).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
