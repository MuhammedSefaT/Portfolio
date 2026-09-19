using FluentValidation;
using Portfolyo.Web.Application.DTOs.UserRoles;

namespace Portfolyo.Web.Application.Validators;

public class UserRoleCreateDtoValidator : AbstractValidator<UserRoleCreateDto>
{
    public UserRoleCreateDtoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Kullanıcı seçilmelidir.");
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Rol seçilmelidir.");
    }
}
