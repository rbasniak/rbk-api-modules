using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Shouldly;
using System.Text.Json;

namespace rbkApiModules.Testing.Core;

/// <summary>
/// Test fixture for Aspire-based E2E tests that manages the distributed application lifecycle
/// and provides browser context with authentication
/// </summary>
public class RbkAspireTestingServer<TAppHost> : IAsyncDisposable where TAppHost: class
{
    private DistributedApplication? _app;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private HttpClient? _httpClient;
    private bool _initialized = false;

    /// <summary>
    /// Controls verbose diagnostic output to the console.
    /// Set by <see cref="RbkPlaywrightTestBase"/> from its <c>VerboseLogging</c> property before each test.
    /// </summary>
    public bool VerboseLogging { get; set; } = false;

    /// <summary>
    /// Controls whether the browser runs in headless mode.
    /// Defaults to the <c>E2E_HEADLESS</c> environment variable (false if not set).
    /// Can be overridden per test class via <see cref="RbkPlaywrightTestBase.Headless"/>.
    /// </summary>
    public bool Headless { get; set; } = GetEnvBool("E2E_HEADLESS", defaultValue: true);

    private void LogDiagnosticMessage(string message)
    {
        if (VerboseLogging) Console.WriteLine(message);
    }

    public DistributedApplication App => _app ?? throw new InvalidOperationException("App not initialized");
    public IBrowser Browser => _browser ?? throw new InvalidOperationException("Browser not initialized");
    public HttpClient HttpClient => _httpClient ?? throw new InvalidOperationException("HttpClient not initialized");
    public string BackendUrl { get; private set; } = string.Empty;
    public string FrontendUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Initialize the distributed application and browser
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            LogDiagnosticMessage("✓ Fixture already initialized, skipping...");
            return;
        }

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
            LogDiagnosticMessage("  - This will start SQL Server, Backend API, and Frontend");
            LogDiagnosticMessage("  - Please wait, this may take 30-60 seconds...");
            await _app.StartAsync();

            LogDiagnosticMessage("✓ Aspire application started successfully!");

            // Create HTTP client for API calls
            LogDiagnosticMessage("Step 4: Creating HTTP client for backend...");
            _httpClient = _app.CreateHttpClient("backend", "https");

            // Get backend URL from the HTTP client - this is the ACTUAL URL assigned by Aspire
            BackendUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/') 
                ?? throw new InvalidOperationException("Failed to get backend URL from Aspire");

            LogDiagnosticMessage($"  - Backend URL (from Aspire): {BackendUrl}");
            LogDiagnosticMessage($"  ⚠ Note: Aspire Testing uses random ports. Angular needs to use this URL!");

            // Frontend URL: from E2E_FRONTEND_URL or default (Program.cs uses port 4207, base path /gcab)
            FrontendUrl = Environment.GetEnvironmentVariable("E2E_FRONTEND_URL")?.TrimEnd('/')
                ?? "http://localhost:4207/gcab";
            LogDiagnosticMessage($"  - Frontend URL: {FrontendUrl}");

            // Initialize Playwright (headless/slow-mo from env: E2E_HEADLESS, E2E_SLOW_MO for CI/local)
            var slowMo = GetEnvInt("E2E_SLOW_MO", defaultValue: 0);
            LogDiagnosticMessage("Step 5: Initializing Playwright browser...");
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new()
            {
                Headless = Headless,
                SlowMo = slowMo,
                Args = new[] { "--ignore-certificate-errors" }
            });
            LogDiagnosticMessage("✓ Playwright browser initialized!");

            // Wait for frontend to be ready
            LogDiagnosticMessage("Step 6: Waiting for frontend to be ready...");
            LogDiagnosticMessage($"  - Frontend should be at: {FrontendUrl}");
            LogDiagnosticMessage("  - This may take 30-60 seconds for Angular to build...");

            var frontendReady = await WaitForFrontendAsync(FrontendUrl, timeout: TimeSpan.FromMinutes(2));
            if (!frontendReady)
            {
                LogDiagnosticMessage("⚠ WARNING: Frontend is not responding after 2 minutes!");
                LogDiagnosticMessage("  - Check if 'npm start' works in the front/ directory");
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

    /// <summary>
    /// Wait for the frontend to be ready by polling the URL
    /// </summary>
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

                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.OK)
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
                LogDiagnosticMessage($"  - Request timed out, retrying...");
            }

            await Task.Delay(3000); // Wait 3 seconds between attempts
        }

        return false;
    }

    /// <summary>
    /// Create a new browser context with authentication pre-configured
    /// </summary>
    public async Task<IBrowserContext> CreateAuthenticatedContextAsync(string username, string tenant, string loginPath = "/api/ca/login")
    {
        // Create context with request interception to redirect API calls
        // This allows Angular to use hardcoded https://localhost:44301 which gets redirected to actual Aspire URL
        var context = await Browser.NewContextAsync(new()
        {
            IgnoreHTTPSErrors = true,
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        });

        // Intercept all requests to the hardcoded backend URL and redirect to actual Aspire URL
        await context.RouteAsync("https://localhost:44301/**", async route =>
        {
            var request = route.Request;
            var url = request.Url.Replace("https://localhost:44301", BackendUrl);

            LogDiagnosticMessage($"Redirecting: {request.Url} → {url}");

            await route.ContinueAsync(new() { Url = url });
        });

        // Create a new page to perform authentication
        var page = await context.NewPageAsync();

        try
        {
            var loginPayload = new { tenant, username };

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

                // Navigate to the frontend to set localStorage
                LogDiagnosticMessage($"Navigating to frontend: {FrontendUrl}");
                await page.GotoAsync(FrontendUrl, new() { Timeout = 60000, WaitUntil = WaitUntilState.NetworkIdle });

                // Set the access token in localStorage
                LogDiagnosticMessage("Setting access token in localStorage");
                await page.EvaluateAsync($@"
                    localStorage.setItem('gcab_access_token', '{accessToken}');
                ");

                // Reload the page to apply authentication
                LogDiagnosticMessage($"Note: API calls to https://localhost:44301 will be automatically redirected to {BackendUrl}");

                // Always navigate to the frontend home page to exit the login screen
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
        if (string.IsNullOrEmpty(value)) return defaultValue;
        return value.Trim().ToLowerInvariant() is "1" or "true" or "yes";
    }

    private static int GetEnvInt(string name, int defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(value)) return defaultValue;
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
