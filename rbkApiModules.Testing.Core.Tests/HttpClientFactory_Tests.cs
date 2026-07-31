using System.Collections.Concurrent;
using System.Net;
using System.Text;
using Shouldly;

namespace rbkApiModules.Testing.Core.Tests;

public class HttpClientFactory_Tests
{
    private interface IProcessingClient;

    [Test]
    public void CreateClient_ThrowsWhenNameIsNotRegistered()
    {
        var factory = new CustomHttpClientFactory(new ConcurrentDictionary<string, HttpClient>());

        var ex = Should.Throw<InvalidOperationException>(() => factory.CreateClient("MissingClient"));

        ex.Message.ShouldContain("MissingClient");
        ex.Message.ShouldContain("was not found");
    }

    [Test]
    public async Task UnboundNamedHttpClient_ThrowsClearMessageWhenUsed()
    {
        var handler = new UnboundNamedHttpClientHandler(nameof(IProcessingClient));
        var client = new HttpClient(handler);

        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            client.GetAsync("http://localhost/api/process"));

        ex.Message.ShouldContain(nameof(IProcessingClient));
        ex.Message.ShouldContain("has not been bound");
        ex.Message.ShouldContain("SetNamedHttpClient");
    }

    [Test]
    public void CustomHttpClientFactory_ReturnsRegisteredClient()
    {
        var clients = new ConcurrentDictionary<string, HttpClient>();
        var expected = new HttpClient { BaseAddress = new Uri("http://localhost") };
        clients["IExternalClient"] = expected;

        var factory = new CustomHttpClientFactory(clients);

        factory.CreateClient("IExternalClient").ShouldBeSameAs(expected);
    }
}
