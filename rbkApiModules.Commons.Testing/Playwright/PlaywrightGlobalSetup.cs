namespace GCAB.Tests.E2E;

/// <summary>
/// Global test session setup for Playwright E2E tests
/// </summary>
public static class PlaywrightGlobalSetup
{
    /// <summary>
    /// Initialize Playwright browsers before any tests run.
    /// Set E2E_SKIP_PLAYWRIGHT_INSTALL=true to skip browser install (e.g. CI or when already installed).
    /// </summary>
    [Before(HookType.TestSession)]
    public static async Task GlobalSetup()
    {
        if (string.Equals(Environment.GetEnvironmentVariable("E2E_SKIP_PLAYWRIGHT_INSTALL"), "true", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Microsoft.Playwright.Program.Main(new[] { "install" });
        await Task.CompletedTask;
    }
}

