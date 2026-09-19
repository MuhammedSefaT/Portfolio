using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Constants;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.Auth;

namespace Portfolyo.Web.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IUserRoleService _userRoleService;
    private readonly IRolePermissionService _rolePermissionService;
    private readonly IEnumerable<IValidator<LoginDto>> _validators;

    public AuthService(
        IUserService userService,
        IUserRoleService userRoleService,
        IRolePermissionService rolePermissionService,
        IEnumerable<IValidator<LoginDto>> validators)
    {
        _userService = userService;
        _userRoleService = userRoleService;
        _rolePermissionService = rolePermissionService;
        _validators = validators;
    }

    public async Task<Result<ClaimsPrincipal>> LoginAsync(
        LoginDto dto, CancellationToken cancellationToken = default)
    {
        foreach (var validator in _validators)
        {
            var validation = await validator.ValidateAsync(dto, cancellationToken);

            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(error => new ResultError(error.PropertyName, error.ErrorMessage))
                    .ToList();

                return Result<ClaimsPrincipal>.Invalid(errors);
            }
        }

        // Parola doğrulaması ve "hesap kapalı" kontrolü UserService'te.
        var verification = await _userService.VerifyPasswordAsync(
            dto.UserNameOrEmail, dto.Password, cancellationToken);

        if (verification.IsFailure)
        {
            return verification.CarryFailure<ClaimsPrincipal>();
        }

        var user = verification.Data!;

        var roles = await _userRoleService.GetByUserAsync(user.Id, cancellationToken);
        var permissions = await _rolePermissionService.GetByUserAsync(user.Id, cancellationToken);

        if (permissions.IsFailure)
        {
            return permissions.CarryFailure<ClaimsPrincipal>();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName)
        };

        if (roles.IsSuccess)
        {
            claims.AddRange(roles.Data!.Select(role => new Claim(ClaimTypes.Role, role.RoleName)));
        }

        // Yetkiler claim olarak yazılır; böylece her istekte dört JSON dosyası okunmaz.
        claims.AddRange(permissions.Data!
            .Select(permission => new Claim(AuthConstants.PermissionClaimType, AuthConstants.ClaimValue(permission))));

        // Son giriş zamanı kaydedilir; başarısız olursa giriş engellenmez.
        await _userService.RecordLoginAsync(user.Id, cancellationToken);

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        return Result<ClaimsPrincipal>.Success(new ClaimsPrincipal(identity));
    }
}
