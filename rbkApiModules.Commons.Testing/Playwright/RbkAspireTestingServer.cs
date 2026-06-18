using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Shouldly;
using System.Text.Json;

namespace rbkApiModules.Testing.Core;

/// <summary>
/// Test fixture for Aspire-based E2E tests that manages the distributed application lifecycle
/// and provides browser context with authentication.
/// Create a project-specific subclass and override <see cref="Options"/>.
/// </summary>
public class RbkAspireTestingServer<TAppHost> : IAsyncDisposable where TAppHost : class
{
    private static readonly TimeSpan ResourceStartupTimeout = TimeSpan.FromMinutes(2);

    private DistributedApplication? _app;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private HttpClient? _httpClient;
    private AspireTestingOptions? _options;
    private bool _initialized = false;

    /// <summary>
    /// Project-level E2E configuration. Override in a fixture subclass.
    /// </summary>
    protected virtual AspireTestingOptions Options => AspireTestingOptions.Default;

    /// <summary>
    /// Controls verbose diagnostic output to the console.
    /// Set by <see cref="RbkPlaywrightTestBase{TAppHost}"/> from <see cref="TestSettings"/> before each test.
    /// </summary>
    public bool VerboseLogging { get; set; } = false;

    /// <summary>
    /// Controls whether the browser runs in headless mode.
    /// Defaults to the <c>E2E_HEADLESS</c> environment variable.
    /// Can be overridden per test class via <see cref="TestSettings.Headless"/>.
    /// </summary>
    public bool Headless { get; set; } = GetEnvBool("E2E_HEADLESS", defaultValue: true);

    private void LogDiagnosticMessage(string message)
    {
        if (VerboseLogging)
        {
            Console.WriteLine(message);
        }
    }

    public DistributedApplication App => _app ?? throw new InvalidOperationException("App not initialized");
    public IBrowser Browser => _browser ?? throw new InvalidOperationException("Browser not initialized");
    public HttpClient HttpClient => _httpClient ?? throw new InvalidOperationException("HttpClient not initialized");
    public string BackendUrl { get; private set; } = string.Empty;
    public string FrontendUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Initialize the distributed application and browser.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            LogDiagnosticMessage("✓ Fixture already initialized, skipping...");
            return;
        }

        _options = Options;

        LogDiagnosticMessage("========================================");
        LogDiagnosticMessage("Starting Aspire E2E Test Initialization");
        LogDiagnosticMessage("========================================");

        try
        {
            LogDiagnosticMessage("Step 1: Creating Aspire application builder...");
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<TAppHost>();

            if (!VerboseLogging)
            {
                appHost.Configuration["Logging:LogLevel:Default"] = "Warning";
            }

            LogDiagnosticMessage("Step 2: Building Aspire application...");
            _app = await appHost.BuildAsync();

            LogDiagnosticMessage("Step 3: Starting Aspire application...");
            LogDiagnosticMessage("  - This will start backend, frontend, and dependent resources");
            LogDiagnosticMessage("  - Please wait, this may take 30-60 seconds...");
            await _app.StartAsync();

            LogDiagnosticMessage("✓ Aspire application started successfully!");

            using var startupCts = new CancellationTokenSource(ResourceStartupTimeout);

            LogDiagnosticMessage($"Step 4: Waiting for backend resource '{_options.BackendResourceName}'...");
            await _app.ResourceNotifications
                .WaitForResourceHealthyAsync(_options.BackendResourceName, startupCts.Token);

            LogDiagnosticMessage("Step 5: Creating HTTP client for backend...");
            _httpClient = _app.CreateHttpClient(_options.BackendResourceName, _options.BackendEndpointName);

            BackendUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/')
                ?? throw new InvalidOperationException(
                    $"Failed to get backend URL from Aspire resource '{_options.BackendResourceName}'.");

            LogDiagnosticMessage($"  - Backend URL (from Aspire): {BackendUrl}");

            LogDiagnosticMessage("Step 6: Resolving frontend URL...");
            FrontendUrl = await ResolveFrontendUrlAsync(_options, startupCts.Token);
            LogDiagnosticMessage($"  - Frontend URL: {FrontendUrl}");

            var slowMo = GetEnvInt("E2E_SLOW_MO", defaultValue: 0);
            LogDiagnosticMessage("Step 7: Initializing Playwright browser...");
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new()
            {
                Headless = Headless,
                SlowMo = slowMo,
                Args = new[] { "--ignore-certificate-errors" }
            });
            LogDiagnosticMessage("✓ Playwright browser initialized!");

            LogDiagnosticMessage("Step 8: Waiting for frontend to be ready...");
            LogDiagnosticMessage($"  - Frontend should be at: {FrontendUrl}");
            LogDiagnosticMessage("  - This may take 30-60 seconds for the frontend to build...");

            var frontendReady = await WaitForFrontendAsync(FrontendUrl, ResourceStartupTimeout);
            if (!frontendReady)
            {
                LogDiagnosticMessage("⚠ WARNING: Frontend is not responding after 2 minutes!");
                LogDiagnosticMessage("  - Check Aspire dashboard for frontend logs");
                LogDiagnosticMessage("  - Tests may fail due to frontend not being available");
            }
            else
            {
                LogDiagnosticMessage("✓ Frontend is ready and responding!");
            }

            _initialized = true;

            LogDiagnosticMessage("========================================");
            LogDiagnosticMessage("✓ E2E Test Environment Ready!");
            LogDiagnosticMessage("========================================");
        }
        catch (Exception ex)
        {
            LogDiagnosticMessage("========================================");
            LogDiagnosticMessage("✗ INITIALIZATION FAILED!");
            LogDiagnosticMessage("========================================");
            LogDiagnosticMessage($"Error: {ex.Message}");
            LogDiagnosticMessage($"Type: {ex.GetType().Name}");
            LogDiagnosticMessage($"Stack trace:\n{ex.StackTrace}");

            if (ex.InnerException != null)
            {
                LogDiagnosticMessage($"\nInner exception: {ex.InnerException.Message}");
                LogDiagnosticMessage($"Inner stack trace:\n{ex.InnerException.StackTrace}");
            }

            throw;
        }
    }

    private async Task<string> ResolveFrontendUrlAsync(AspireTestingOptions options, CancellationToken cancellationToken)
    {
        string baseUrl;

        if (!string.IsNullOrWhiteSpace(options.FrontendUrlOverride))
        {
            baseUrl = options.FrontendUrlOverride.TrimEnd('/');
        }
        else
        {
            LogDiagnosticMessage($"  - Waiting for frontend resource '{options.FrontendResourceName}'...");
            await _app!.ResourceNotifications
                .WaitForResourceHealthyAsync(options.FrontendResourceName, cancellationToken);

            var frontendClient = _app.CreateHttpClient(options.FrontendResourceName, options.FrontendEndpointName);
            baseUrl = frontendClient.BaseAddress?.ToString().TrimEnd('/')
                ?? throw new InvalidOperationException(
                    $"Failed to get frontend URL from Aspire resource '{options.FrontendResourceName}'. " +
                    $"Set {nameof(AspireTestingOptions.FrontendUrlOverride)} if the frontend runs outside Aspire.");
        }

        return AppendFrontendBasePath(baseUrl, options.FrontendBasePath);
    }

    private static string AppendFrontendBasePath(string baseUrl, string? basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
        {
            return baseUrl;
        }

        var normalizedPath = basePath.Trim('/');
        return $"{baseUrl}/{normalizedPath}";
    }

    private async Task<bool> WaitForFrontendAsync(string url, TimeSpan timeout)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var client = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true });
        var attempt = 0;

        while (stopwatch.Elapsed < timeout)
        {
            attempt++;
            try
            {
                LogDiagnosticMessage($"  - Attempt {attempt}: Checking if frontend is responding...");
                var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    LogDiagnosticMessage($"  - ✓ Frontend responded with status: {response.StatusCode}");
                    return true;
                }

                LogDiagnosticMessage($"  - Frontend responded but with status: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                LogDiagnosticMessage($"  - Not ready yet: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                LogDiagnosticMessage("  - Request timed out, retrying...");
            }

            await Task.Delay(3000);
        }

        return false;
    }

    /// <summary>
    /// Create a new browser context with authentication pre-configured.
    /// </summary>
    public async Task<IBrowserContext> CreateAuthenticatedContextAsync(string username, string tenant)
    {
        var options = _options ?? Options;

        var context = await Browser.NewContextAsync(new()
        {
            IgnoreHTTPSErrors = true,
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        });

        if (!string.IsNullOrWhiteSpace(options.ApiRedirectOrigin))
        {
            var redirectOrigin = options.ApiRedirectOrigin.TrimEnd('/');
            await context.RouteAsync($"{redirectOrigin}/**", async route =>
            {
                var request = route.Request;
                var url = request.Url.Replace(redirectOrigin, BackendUrl);

                LogDiagnosticMessage($"Redirecting: {request.Url} → {url}");

                await route.ContinueAsync(new() { Url = url });
            });
        }

        var page = await context.NewPageAsync();

        try
        {
            var loginPayload = new { tenant, username };
            var loginPath = options.LoginPath;

            LogDiagnosticMessage($"Logging in via: {BackendUrl}{loginPath}");
            var response = await page.APIRequest.PostAsync($"{BackendUrl}{loginPath}", new()
            {
                DataObject = loginPayload
            });

            response.Ok.ShouldBeTrue($"Login failed with status {response.Status}");

            var responseBody = await response.TextAsync();
            var loginResponse = JsonDocument.Parse(responseBody);

            var accessToken = loginResponse.RootElement.GetProperty("accessToken").GetString();
            accessToken.ShouldNotBeNullOrWhiteSpace("Access token should not be null or empty");

            LogDiagnosticMessage($"Navigating to frontend: {FrontendUrl}");
            await page.GotoAsync(FrontendUrl, new() { Timeout = 60000, WaitUntil = WaitUntilState.NetworkIdle });

            LogDiagnosticMessage($"Setting access token in localStorage key '{options.AccessTokenStorageKey}'");
            await page.EvaluateAsync(
                "(args) => localStorage.setItem(args.key, args.token)",
                new { key = options.AccessTokenStorageKey, token = accessToken });

            if (!string.IsNullOrWhiteSpace(options.ApiRedirectOrigin))
            {
                LogDiagnosticMessage(
                    $"Note: API calls to {options.ApiRedirectOrigin} will be automatically redirected to {BackendUrl}");
            }

            LogDiagnosticMessage("Navigating back to frontend root to leave login flow");
            await page.GotoAsync(FrontendUrl, new() { Timeout = 60000, WaitUntil = WaitUntilState.NetworkIdle });
            LogDiagnosticMessage($"✓ Confirmed frontend navigation. Current URL: {page.Url}");
        }
        finally
        {
            await page.CloseAsync();
        }

        return context;
    }

    private static bool GetEnvBool(string name, bool defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        return value.Trim().ToLowerInvariant() is "1" or "true" or "yes";
    }

    private static int GetEnvInt(string name, int defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        return int.TryParse(value, out var n) ? n : defaultValue;
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser != null)
        {
            await _browser.DisposeAsync();
        }

        if (_playwright != null)
        {
            _playwright.Dispose();
        }

        if (_app != null)
        {
            await _app.DisposeAsync();
        }
    }
}
