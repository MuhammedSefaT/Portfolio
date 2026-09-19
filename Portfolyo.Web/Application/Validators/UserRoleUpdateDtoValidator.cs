using FluentValidation;
using Portfolyo.Web.Application.DTOs.UserRoles;

namespace Portfolyo.Web.Application.Validators;

public class UserRoleUpdateDtoValidator : AbstractValidator<UserRoleUpdateDto>
{
    public UserRoleUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Atama kimliği zorunludur.");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Kullanıcı seçilmelidir.");
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Rol seçilmelidir.");
    }
}
