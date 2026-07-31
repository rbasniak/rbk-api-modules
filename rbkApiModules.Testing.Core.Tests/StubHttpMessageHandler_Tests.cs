using System.Net;
using System.Text;
using Shouldly;

namespace rbkApiModules.Testing.Core.Tests;

public class StubHttpMessageHandler_Tests
{
    private interface IExternalClient;

    [Test]
    public async Task SendAsync_ThrowsWhenNoScopeIsActive()
    {
        var handler = new StubHttpMessageHandler(typeof(IExternalClient));
        var client = new HttpClient(handler);

        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            client.GetAsync("http://localhost/resource"));

        ex.Message.ShouldContain(nameof(HttpMockScope));
    }

    [Test]
    public async Task SendAsync_ReturnsConfiguredResponseInsideScope()
    {
        var handler = new StubHttpMessageHandler(typeof(IExternalClient));

        using var _ = HttpMockScope.Begin();
        new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, url: null)
            .ReturnsSuccess("payload", "text/plain");

        var client = new HttpClient(handler);
        var response = await client.GetAsync("http://localhost/resource");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldBe("payload");
    }

    [Test]
    public async Task SendAsync_ThrowsWhenNoRuleMatches()
    {
        var handler = new StubHttpMessageHandler(typeof(IExternalClient));

        using var _ = HttpMockScope.Begin();
        new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, "/expected")
            .ReturnsSuccess("payload");

        var client = new HttpClient(handler);
        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            client.GetAsync("http://localhost/other"));

        ex.Message.ShouldContain("No HTTP mock rule matched");
        ex.Message.ShouldContain("Registered rules:");
    }

    [Test]
    public async Task SendAsync_IsolatesRulesBetweenNestedScopes()
    {
        var handler = new StubHttpMessageHandler(typeof(IExternalClient));
        var client = new HttpClient(handler);

        using var outer = HttpMockScope.Begin();
        new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, url: null)
            .ReturnsSuccess("outer", "text/plain");

        (await client.GetAsync("http://localhost/a")).Content!
            .ReadAsStringAsync().GetAwaiter().GetResult().ShouldBe("outer");

        using (var inner = HttpMockScope.Begin())
        {
            new HttpMockCallBuilder(typeof(IExternalClient), HttpMethod.Get, url: null)
                .ReturnsSuccess("inner", "text/plain");

            (await client.GetAsync("http://localhost/b")).Content!
                .ReadAsStringAsync().GetAwaiter().GetResult().ShouldBe("inner");
        }

        (await client.GetAsync("http://localhost/c")).Content!
            .ReadAsStringAsync().GetAwaiter().GetResult().ShouldBe("outer");
    }
}
