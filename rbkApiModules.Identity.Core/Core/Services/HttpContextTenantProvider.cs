using Microsoft.AspNetCore.Http;
using rbkApiModules.Commons.Core.Authentication;

namespace rbkApiModules.Identity.Core;

/// <summary>
/// Resolves the current tenant ID from HttpContext JWT claims.
/// </summary>
internal sealed class HttpContextTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public string? CurrentTenantId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var tenantClaim = httpContext.User?.Claims
                .FirstOrDefault(c => c.Type == JwtClaimIdentifiers.Tenant);

            if (tenantClaim == null || string.IsNullOrWhiteSpace(tenantClaim.Value))
            {
                return null;
            }

            return tenantClaim.Value.ToUpperInvariant();
        }
    }
}
