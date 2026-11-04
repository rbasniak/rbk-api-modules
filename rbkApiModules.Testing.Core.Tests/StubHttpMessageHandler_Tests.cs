using System.Net;
using System.Threading;
using Shouldly;

namespace rbkApiModules.Testing.Core.Tests;

public class StubHttpMessageHandler_Tests
{
    private interface IExternalClient;

    private static StubHttpMessageHandler CreateHandler(BackgroundHttpMockScope.RbkTestingServerHolder? holder = null) =>
        new(typeof(IExternalClient), () => holder?.ActiveRules);

    [Test]
    public async Task SendAsync_ThrowsWhenNoScopeIsActive()
    {
        var handler = CreateHandler();
        var client = new HttpClient(handler);

        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            client.GetAsync("http://localhost/resource"));

        ex.Message.ShouldContain(nameof(HttpMockScope));
    }

    [Test]
    public async Task SendAsync_ReturnsConfiguredResponseInsideScope()
    {
        var handler = CreateHandler();

        using var _ = HttpMockScope.Begin();
        new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, urlMatcher: null)
            .ReturnsSuccess("payload", "text/plain");

        var client = new HttpClient(handler);
        var response = await client.GetAsync("http://localhost/resource");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldBe("payload");
    }

    [Test]
    public async Task SendAsync_ThrowsWhenNoRuleMatches()
    {
        var handler = CreateHandler();

        using var _ = HttpMockScope.Begin();
        new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, url => url.Contains("/expected"))
            .ReturnsSuccess("payload");

        var client = new HttpClient(handler);
        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            client.GetAsync("http://localhost/other"));

        ex.Message.ShouldContain("No HTTP mock rule matched");
        ex.Message.ShouldContain("Registered rules:");
    }

    [Test]
    public async Task SendAsync_MatchesUrlPredicate()
    {
        var handler = CreateHandler();

        using var _ = HttpMockScope.Begin();
        new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, url => url.Contains("target-doc"))
            .ReturnsSuccess("matched", "text/plain");

        var client = new HttpClient(handler);
        var response = await client.GetAsync("http://localhost/files/target-doc.pdf");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldBe("matched");
    }

    [Test]
    public async Task SendAsync_IsolatesRulesBetweenNestedScopes()
    {
        var handler = CreateHandler();
        var client = new HttpClient(handler);

        using var outer = HttpMockScope.Begin();
        new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, urlMatcher: null)
            .ReturnsSuccess("outer", "text/plain");

        (await client.GetAsync("http://localhost/a")).Content!
            .ReadAsStringAsync().GetAwaiter().GetResult().ShouldBe("outer");

        using (var inner = HttpMockScope.Begin())
        {
            new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, urlMatcher: null)
                .ReturnsSuccess("inner", "text/plain");

            (await client.GetAsync("http://localhost/b")).Content!
                .ReadAsStringAsync().GetAwaiter().GetResult().ShouldBe("inner");
        }

        (await client.GetAsync("http://localhost/c")).Content!
            .ReadAsStringAsync().GetAwaiter().GetResult().ShouldBe("outer");
    }

    [Test]
    public async Task SendAsync_UsesBackgroundScopeWhenExecutionContextDoesNotFlow()
    {
        var holder = new BackgroundHttpMockScope.RbkTestingServerHolder();
        var handler = CreateHandler(holder);

        using var background = new BackgroundHttpMockScope(holder);
        new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, urlMatcher: null, () => holder.ActiveRules)
            .ReturnsSuccess("from-background", "text/plain");

        var client = new HttpClient(handler);
        HttpResponseMessage? response = null;

        ExecutionContext.SuppressFlow();
        try
        {
            response = await client.GetAsync("http://localhost/resource");
        }
        finally
        {
            ExecutionContext.RestoreFlow();
        }

        response!.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldBe("from-background");
    }

    [Test]
    public async Task SendAsync_PrefersRequestScopeOverBackgroundWhenBothConfigured()
    {
        var holder = new BackgroundHttpMockScope.RbkTestingServerHolder();
        var handler = CreateHandler(holder);

        using var background = new BackgroundHttpMockScope(holder);
        new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, urlMatcher: null, () => holder.ActiveRules)
            .ReturnsSuccess("background", "text/plain");

        using var requestScope = HttpMockScope.Begin();
        new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, urlMatcher: null)
            .ReturnsSuccess("request", "text/plain");

        var client = new HttpClient(handler);
        var response = await client.GetAsync("http://localhost/resource");

        (await response.Content.ReadAsStringAsync()).ShouldBe("request");
    }

    [Test]
    public async Task SendAsync_DoesNotUseBackgroundRulesAfterBackgroundScopeDisposed()
    {
        var holder = new BackgroundHttpMockScope.RbkTestingServerHolder();
        var handler = CreateHandler(holder);

        using (var background = new BackgroundHttpMockScope(holder))
        {
            new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, urlMatcher: null, () => holder.ActiveRules)
                .ReturnsSuccess("from-background", "text/plain");
        }

        var client = new HttpClient(handler);

        ExecutionContext.SuppressFlow();
        try
        {
            await Should.ThrowAsync<InvalidOperationException>(() => client.GetAsync("http://localhost/resource"));
        }
        finally
        {
            if (ExecutionContext.IsFlowSuppressed())
            {
                ExecutionContext.RestoreFlow();
            }
        }
    }
}
