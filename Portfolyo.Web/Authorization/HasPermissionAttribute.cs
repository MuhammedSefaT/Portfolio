using Microsoft.AspNetCore.Authorization;
using Portfolyo.Web.Application.Common.Constants;
using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Authorization;

/// <summary>
/// Controller veya action'ı belirli bir yetkiyle sınırlar.
/// Kullanımı: [HasPermission(Permission.UserCreate)]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(Permission permission)
        => Policy = AuthConstants.PolicyName(permission);
}
