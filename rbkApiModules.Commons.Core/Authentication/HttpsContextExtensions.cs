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

    public static string GetUsername(this HttpContext httpContext)
    {
        if (httpContext == null || httpContext.User == null || httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
        {
            return string.Empty;
        }

        var username = httpContext!.User!.Identity!.Name;

        return username?.ToLower() ?? string.Empty;
    }

    public static string GetTenant(this HttpContext httpContext)
    {
        if (httpContext == null || httpContext.User == null || httpContext.User.Claims == null || httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
        {
            return string.Empty;
        }

        var tenantClaim = httpContext.User.Claims.FirstOrDefault(x => x.Type == JwtClaimIdentifiers.Tenant);

        if (tenantClaim == null)
        {
            return string.Empty;
        }

        return String.IsNullOrEmpty(tenantClaim.Value) ? string.Empty : tenantClaim.Value;
    }

}