using System.Collections.Concurrent;

namespace rbkApiModules.Commons.Testing;

public sealed class CustomHttpClientFactory : IHttpClientFactory
{
    private readonly ConcurrentDictionary<string, HttpClient> _clients;

    public CustomHttpClientFactory(ConcurrentDictionary<string, HttpClient> clients)
    {
        _clients = clients;
    }

    public HttpClient CreateClient(string name) =>
        _clients.TryGetValue(name, out var client)
            ? client
            : throw new InvalidOperationException($"HttpClient \"{name}\" was not found");
}
