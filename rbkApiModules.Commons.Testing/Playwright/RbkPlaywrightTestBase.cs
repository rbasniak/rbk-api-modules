namespace rbkApiModules.Testing.Core;

/// <summary>
/// Base class for E2E tests providing common functionality for Playwright tests
/// </summary>
public abstract class RbkPlaywrightTestBase<TAppHost> : IAsyncDisposable where TAppHost : class
{
    protected IBrowserContext? Context;
    protected IPage? Page;

    /// <summary>
    /// Injected by derived class via ClassDataSource; derived classes must apply the [ClassDataSource] attribute
    /// with the concrete type.
    /// </summary>
    public abstract RbkAspireTestingServer<TAppHost> Fixture { get; set; }

    /// <summary>
    /// Artifacts directory (screenshots, logs); resolved from test output directory so CI and local runs are consistent.
    /// </summary>
    protected static string ArtifactsDirectory
    {
        get
        {
            var baseDir = AppContext.BaseDirectory;
            var dir = Path.Combine(string.IsNullOrEmpty(baseDir) ? Directory.GetCurrentDirectory() : baseDir, "artifacts");
            Directory.CreateDirectory(dir);
            return dir;
        }
    }

    protected virtual TestSettings TestSettings => TestSettings.Default;

    /// <summary>
    /// Login endpoint path used by <see cref="Authenticate"/>.
    /// Defaults to <c>/api/ca/login</c>. Override in a test class to point at a different login route.
    /// </summary>
    protected virtual string LoginPath => "/api/authentication/login";

    private void LogDiagnosticMessage(string message)
    {
        if (TestSettings.EnableAspireDiagnosticMessages)
        {
            Console.WriteLine(message);
        }
    }

    /// <summary>
    /// Set up test - creates authenticated browser context and page
    /// </summary>
    [Before(HookType.Test)]
    public virtual async Task SetUp()
    {
        LogDiagnosticMessage($"\n=== Setting up test: {TestContext.Current?.Metadata?.TestName ?? "unknown"} ===");

        // Ensure fixture is initialized
        LogDiagnosticMessage("Initializing Aspire fixture...");

        Fixture.VerboseLogging = TestSettings.EnableAspireDiagnosticMessages;

        if (TestSettings.Headless.HasValue)
        {
            Fixture.Headless = TestSettings.Headless.Value;
        }

        await Fixture.InitializeAsync();
    }

    /// <summary>
    /// Tear down test - captures screenshot on failure when detectable, and cleans up resources.
    /// When TUnit exposes test result in After (e.g. TestContext.Result), gate screenshot on failure only.
    /// </summary>
    [After(HookType.Test)]
    public virtual async Task TearDown()
    {
        var testContext = TestContext.Current;
        // Prefer screenshot only on failure; current TUnit/rbkApiModules may not expose Result on TestContext
        var captureScreenshot = ShouldCaptureScreenshot(testContext);

        if (Page != null && captureScreenshot)
        {
            var name = testContext?.Metadata?.TestName ?? "unknown";
            var screenshotPath = Path.Combine(ArtifactsDirectory, $"screenshot-{name}-{Guid.NewGuid()}.png");

            try
            {
                await Page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });
                LogDiagnosticMessage($"Screenshot saved to: {screenshotPath}");
            }
            catch (Exception ex)
            {
                LogDiagnosticMessage($"Failed to capture screenshot: {ex.Message}");
            }
        }

        await DisposeAsync();
    }

    protected async Task Authenticate(string username, string tenant)
    {
        LogDiagnosticMessage("Creating authenticated browser context...");
        Context = await Fixture.CreateAuthenticatedContextAsync(username, tenant, LoginPath);
        Page = await Context.NewPageAsync();

        // Navigate to the frontend
        LogDiagnosticMessage($"Navigating to: {Fixture.FrontendUrl}");
        await Page.GotoAsync(Fixture.FrontendUrl);
        LogDiagnosticMessage("✓ Test setup complete!\n");
    }

    /// <summary>
    /// Override to gate screenshots on test failure when the test framework exposes result (e.g. TestContext.Result).
    /// Default: capture when E2E_SCREENSHOT_ALWAYS is set, otherwise assume failure (capture) so we don't miss failures.
    /// </summary>
    protected virtual bool ShouldCaptureScreenshot(TestContext? testContext)
    {
        var state = testContext.Execution.Result.State;

        var isEnabledInEnvironment = string.Equals(Environment.GetEnvironmentVariable("E2E_SCREENSHOT_ALWAYS"), "true", StringComparison.OrdinalIgnoreCase);
        var testFailed = state == TestState.Failed || state == TestState.Timeout;

        return isEnabledInEnvironment || testFailed;
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (Page != null)
        {
            await Page.CloseAsync();
        }

        if (Context != null)
        {
            await Context.CloseAsync();
        }
    }
}


public sealed class TestSettings
{
    /// <summary>
    /// Override to <c>true</c> in a test class to enable verbose diagnostic output to the console.
    /// Useful when debugging a failing test locally; leave as <c>false</c> (the default) for normal runs.
    /// </summary>
    public bool EnableAspireDiagnosticMessages { get; init; } = false;

    /// <summary>
    /// Override in a test class to force the browser to run in headless or headed mode, regardless of the
    /// <c>E2E_HEADLESS</c> environment variable. Return <c>null</c> (the default) to let the environment
    /// variable decide.
    /// </summary>
    public bool? Headless { get; init; } = null;

    /// <summary>
    /// Login endpoint path used by <see cref="RbkPlaywrightTestBase.Authenticate"/>.
    /// Defaults to <c>/api/ca/login</c>. Override in a test class to point at a different login route.
    /// </summary>
    public string LoginPath { get; init; } = "/api/authentication/login";

    public static TestSettings Default => new TestSettings();

    public static TestSettings Debug => new TestSettings { Headless = false, EnableAspireDiagnosticMessages = true };

}