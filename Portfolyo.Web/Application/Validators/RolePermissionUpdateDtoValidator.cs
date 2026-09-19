using FluentValidation;
using Portfolyo.Web.Application.DTOs.RolePermissions;

namespace Portfolyo.Web.Application.Validators;

public class RolePermissionUpdateDtoValidator : AbstractValidator<RolePermissionUpdateDto>
{
    public RolePermissionUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Kayıt kimliği zorunludur.");
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Rol seçilmelidir.");
        RuleFor(x => x.Permission).IsInEnum().WithMessage("Tanımlı olmayan bir yetki gönderildi.");
    }
}
