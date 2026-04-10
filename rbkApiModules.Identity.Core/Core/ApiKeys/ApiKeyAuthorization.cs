namespace rbkApiModules.Identity.Core;

internal static class ApiKeyAuthorization
{
    public static bool CallerHasCrossTenantApiKeyClaim(AuthenticatedUser identity)
    {
        return identity.HasClaim(AuthenticationClaims.CAN_MANAGE_CROSS_TENANT_API_KEYS);
    }

    /// <summary>
    /// Restricts which API key rows a caller may see when listing, if they lack <see cref="AuthenticationClaims.CAN_MANAGE_CROSS_TENANT_API_KEYS"/>.
    /// </summary>
    /// <remarks>
    /// The list endpoint is intended for JWT only: global admins (cross-tenant claim) see all keys;
    /// tenant users see only keys for <see cref="AuthenticatedUser.Tenant"/>. If there is no cross-tenant
    /// claim and no tenant on the identity, nothing is returned (defensive; not expected for normal JWT callers).
    /// </remarks>
    public static IQueryable<ApiKey> FilterApiKeysForListByCallerScope(IQueryable<ApiKey> query, AuthenticatedUser identity)
    {
        if (CallerHasCrossTenantApiKeyClaim(identity))
        {
            return query;
        }

        if (string.IsNullOrEmpty(identity.Tenant))
        {
            return query.Where(x => false);
        }

        var tenant = identity.Tenant;
        return query.Where(x => x.TenantId == tenant);
    }

    /// <summary>
    /// Whether the caller may list, update, or revoke a key with the given persisted <see cref="ApiKey.TenantId"/>.
    /// Cross-tenant claim grants access to all keys; otherwise the key must fall in the caller's tenant slice.
    /// </summary>
    public static bool CanManageExistingKeyInScope(AuthenticatedUser identity, string? keyTenantId)
    {
        if (CallerHasCrossTenantApiKeyClaim(identity))
        {
            return true;
        }

        var callerEmpty = string.IsNullOrEmpty(identity.Tenant);
        var keyNormalized = NormalizeKeyTenant(keyTenantId);
        var keyEmpty = keyNormalized == null;

        if (callerEmpty && keyEmpty)
        {
            return true;
        }

        if (!callerEmpty && !keyEmpty && string.Equals(identity.Tenant, keyNormalized, StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    private static string? NormalizeKeyTenant(string? tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return null;
        }

        return tenantId.Trim().ToUpperInvariant();
    }
}
