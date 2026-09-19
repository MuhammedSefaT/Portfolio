using FluentValidation;
using Portfolyo.Web.Application.DTOs.RolePermissions;

namespace Portfolyo.Web.Application.Validators;

public class RolePermissionCreateDtoValidator : AbstractValidator<RolePermissionCreateDto>
{
    public RolePermissionCreateDtoValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Rol seçilmelidir.");
        RuleFor(x => x.Permission).IsInEnum().WithMessage("Tanımlı olmayan bir yetki gönderildi.");
    }
}
