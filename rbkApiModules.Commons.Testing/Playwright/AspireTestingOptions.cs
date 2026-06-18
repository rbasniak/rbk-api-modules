namespace rbkApiModules.Testing.Core;

/// <summary>
/// Project-level E2E configuration for <see cref="RbkAspireTestingServer{TAppHost}"/>.
/// Override via a fixture subclass; these values are constant for the entire test project.
/// </summary>
public sealed class AspireTestingOptions
{
    /// <summary>
    /// Aspire resource name for the backend API (must match the AppHost registration).
    /// The backend must expose an HTTPS endpoint named <c>https</c> (e.g. <c>.WithHttpsEndpoint(port: 44301, name: "https")</c>).
    /// </summary>
    public string BackendResourceName { get; init; } = "backend";

    /// <summary>
    /// Aspire resource name for the frontend (must match the AppHost registration).
    /// Used to wait for the frontend process to become healthy before tests run.
    /// </summary>
    public string FrontendResourceName { get; init; } = "frontend";

    /// <summary>
    /// Localhost port where the frontend dev server listens during E2E tests (e.g. <c>4207</c> for ng serve).
    /// The fixture builds the URL as <c>http://localhost:{FrontendPort}</c>.
    /// </summary>
    public int FrontendPort { get; init; } = 4207;

    /// <summary>
    /// Optional path suffix appended to the frontend base URL (e.g. <c>/gcab</c> for a base href).
    /// </summary>
    public string? FrontendBasePath { get; init; }

    /// <summary>
    /// localStorage key used by the frontend to store the JWT access token.
    /// </summary>
    public string AccessTokenStorageKey { get; init; } = "access_token";

    /// <summary>
    /// Backend login endpoint path (e.g. <c>/api/authentication/login</c>).
    /// </summary>
    public string LoginPath { get; init; } = "/api/authentication/login";

    public static AspireTestingOptions Default { get; } = new();
}
