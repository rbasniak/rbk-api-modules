using Microsoft.AspNetCore.Http;

namespace rbkApiModules.Commons.Core;

public static class HttpContextExtensions
{
    public static string GetUsername(this IHttpContextAccessor httpContextAccessor)
    {
        return httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
    }

    public static string GetTenant(this IHttpContextAccessor httpContextAccessor)
    {
        var tenant = httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == JwtClaimIdentifiers.Tenant).Value;

        return String.IsNullOrEmpty(tenant) ? null : tenant;
    }
}