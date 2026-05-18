using FluentValidation;
using Portfolio.Common.Dtos;

namespace Portfolio.Business.ValidationRules;

public class CertificateCreateDtoValidator : AbstractValidator<CertificateCreateDto>
{
    public CertificateCreateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Issuer).NotEmpty().MaximumLength(250);
        RuleFor(x => x.VerificationUrl).MaximumLength(500);
        RuleFor(x => x.ImageUrl).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
