namespace rbkApiModules.Testing.Core;

/// <summary>
/// Project-level E2E configuration for <see cref="RbkAspireTestingServer{TAppHost}"/>.
/// Override via a fixture subclass; these values are constant for the entire test project.
/// </summary>
public sealed class AspireTestingOptions
{
    /// <summary>
    /// Aspire resource name for the backend API (must match the AppHost registration).
    /// </summary>
    public string BackendResourceName { get; init; } = "backend";

    /// <summary>
    /// Endpoint name on the backend resource (typically <c>https</c>).
    /// </summary>
    public string BackendEndpointName { get; init; } = "https";

    /// <summary>
    /// Aspire resource name for the frontend (must match the AppHost registration).
    /// </summary>
    public string FrontendResourceName { get; init; } = "frontend";

    /// <summary>
    /// Endpoint name on the frontend resource (typically <c>https</c> or <c>http</c>).
    /// </summary>
    public string FrontendEndpointName { get; init; } = "https";

    /// <summary>
    /// Optional path suffix appended to the frontend base URL (e.g. <c>/gcab</c> for a base href).
    /// </summary>
    public string? FrontendBasePath { get; init; }

    /// <summary>
    /// Fixed frontend URL used when the frontend does not run as an Aspire resource (local debug).
    /// When set, Aspire frontend lookup is skipped.
    /// </summary>
    public string? FrontendUrlOverride { get; init; }

    /// <summary>
    /// localStorage key used by the frontend to store the JWT access token.
    /// </summary>
    public string AccessTokenStorageKey { get; init; } = "access_token";

    /// <summary>
    /// Origin that the frontend hardcodes for API calls (e.g. <c>https://localhost:44301</c>).
    /// When set, Playwright intercepts requests to this origin and redirects them to the actual Aspire backend URL.
    /// </summary>
    public string? ApiRedirectOrigin { get; init; }

    /// <summary>
    /// Backend login endpoint path (e.g. <c>/api/authentication/login</c>).
    /// </summary>
    public string LoginPath { get; init; } = "/api/authentication/login";

    public static AspireTestingOptions Default { get; } = new();
}
