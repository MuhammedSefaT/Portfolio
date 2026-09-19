using FluentValidation;
using Portfolyo.Web.Application.DTOs.Users;

namespace Portfolyo.Web.Application.Validators;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Kullanıcı kimliği zorunludur.");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Kullanıcı adı zorunludur.")
            .Length(3, 50).WithMessage("Kullanıcı adı 3-50 karakter olmalıdır.")
            .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Kullanıcı adı yalnızca harf, rakam, nokta, alt çizgi ve tire içerebilir.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
            .MaximumLength(150).WithMessage("E-posta en fazla 150 karakter olabilir.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad zorunludur.")
            .MaximumLength(75).WithMessage("Ad en fazla 75 karakter olabilir.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad zorunludur.")
            .MaximumLength(75).WithMessage("Soyad en fazla 75 karakter olabilir.");

        RuleFor(x => x.ProfileImagePath)
            .MaximumLength(260).WithMessage("Görsel yolu en fazla 260 karakter olabilir.");
    }
}
