using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.Auth;
using Portfolyo.Web.Extensions;

namespace Portfolyo.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService) => _authService = authService;

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        // Girişi yapılmış kullanıcı formu tekrar görmesin.
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        ViewData["ReturnUrl"] = returnUrl;

        return View(new LoginDto(string.Empty, string.Empty));
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        ViewData["ReturnUrl"] = returnUrl;

        var result = await _authService.LoginAsync(model, cancellationToken);

        if (result.IsFailure)
        {
            // Parola hataları alan bazlı gösterilmesin; formun genel hatası olarak dursun.
            if (result.Status is ResultStatus.ValidationError && result.Errors.Count > 0)
            {
                ModelState.AddModelError(string.Empty, result.Errors[0].Message);
            }
            else
            {
                result.AddToModelState(ModelState);
            }

            return View(model with { Password = string.Empty });
        }

        var properties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : null
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, result.Data!, properties);

        return RedirectToLocal(returnUrl);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    /// <summary>
    /// Yalnızca site içi adreslere yönlendirir; açık yönlendirme (open redirect) engellenir.
    /// </summary>
    private IActionResult RedirectToLocal(string? returnUrl)
        => !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction(nameof(HomeController.Index), "Home");
}
