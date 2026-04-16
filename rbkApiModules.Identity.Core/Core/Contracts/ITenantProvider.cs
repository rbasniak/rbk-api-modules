namespace rbkApiModules.Identity.Core;

/// <summary>
/// Provides the current tenant identifier for query filter resolution.
/// Resolved automatically from HttpContext JWT claims by the library.
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// Gets the current tenant ID. Returns null if no tenant context (unauthenticated or global context).
    /// Re-evaluated on every access — never cached.
    /// </summary>
    string? CurrentTenantId { get; }
}
