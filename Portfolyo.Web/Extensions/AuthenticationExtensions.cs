using Microsoft.AspNetCore.Authentication.Cookies;
using Portfolyo.Web.Application.Common.Constants;
using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Extensions;

public static class AuthenticationExtensions
{
    /// <summary>
    /// Çerez tabanlı kimlik doğrulama ve her Permission değeri için bir yetki politikası ekler.
    /// </summary>
    public static IServiceCollection AddCookieAuthentication(
        this IServiceCollection services, IHostEnvironment environment)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = AuthConstants.CookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;

                // Geliştirmede http profili de çalışsın; yayında yalnızca https.
                options.Cookie.SecurePolicy = environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;

                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ReturnUrlParameter = "returnUrl";

                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });

        services.AddAuthorization(options =>
        {
            // Her yetki için bir politika: [HasPermission(Permission.UserView)] bunu kullanır.
            foreach (var permission in Enum.GetValues<Permission>().Where(x => x != Permission.None))
            {
                options.AddPolicy(
                    AuthConstants.PolicyName(permission),
                    policy => policy
                        .RequireAuthenticatedUser()
                        .RequireClaim(AuthConstants.PermissionClaimType, AuthConstants.ClaimValue(permission)));
            }
        });

        return services;
    }
}
