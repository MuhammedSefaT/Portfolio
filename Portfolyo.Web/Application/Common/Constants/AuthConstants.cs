using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Application.Common.Constants;

/// <summary>
/// Kimlik doğrulama ve yetkilendirmede kullanılan sabit adlar.
/// </summary>
public static class AuthConstants
{
    /// <summary>Yetkilerin claim olarak yazıldığı tür adı.</summary>
    public const string PermissionClaimType = "portfolyo:permission";

    /// <summary>Oturum çerezinin adı.</summary>
    public const string CookieName = "Portfolyo.Auth";

    private const string PolicyPrefix = "Permission:";

    /// <summary>Permission.UserView -> "Permission:UserView"</summary>
    public static string PolicyName(Permission permission) => PolicyPrefix + permission;

    /// <summary>Claim değeri olarak enum'un sayısal karşılığı yazılır.</summary>
    public static string ClaimValue(Permission permission) => ((int)permission).ToString();
}
