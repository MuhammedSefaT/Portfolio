using FluentValidation;
using Portfolyo.Web.Application.DTOs.Roles;

namespace Portfolyo.Web.Application.Validators;

public class RoleCreateDtoValidator : AbstractValidator<RoleCreateDto>
{
    public RoleCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Rol adı zorunludur.")
            .Length(2, 50).WithMessage("Rol adı 2-50 karakter olmalıdır.");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Açıklama en fazla 250 karakter olabilir.");
    }
}
