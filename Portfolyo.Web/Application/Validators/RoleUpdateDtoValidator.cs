using FluentValidation;
using Portfolyo.Web.Application.DTOs.Roles;

namespace Portfolyo.Web.Application.Validators;

public class RoleUpdateDtoValidator : AbstractValidator<RoleUpdateDto>
{
    public RoleUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Rol kimliği zorunludur.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Rol adı zorunludur.")
            .Length(2, 50).WithMessage("Rol adı 2-50 karakter olmalıdır.");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Açıklama en fazla 250 karakter olabilir.");
    }
}
