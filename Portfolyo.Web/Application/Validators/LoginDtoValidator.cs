using FluentValidation;
using Portfolyo.Web.Application.DTOs.Auth;

namespace Portfolyo.Web.Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.UserNameOrEmail)
            .NotEmpty().WithMessage("Kullanıcı adı veya e-posta zorunludur.")
            .MaximumLength(150).WithMessage("Girilen değer çok uzun.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parola zorunludur.")
            .MaximumLength(128).WithMessage("Parola çok uzun.");
    }
}
