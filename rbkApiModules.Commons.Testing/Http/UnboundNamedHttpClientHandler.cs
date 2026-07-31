namespace rbkApiModules.Commons.Testing;

internal sealed class UnboundNamedHttpClientHandler : HttpMessageHandler
{
    private readonly string _name;

    public UnboundNamedHttpClientHandler(string name)
    {
        _name = name;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        throw new InvalidOperationException(
            $"Named HttpClient \"{_name}\" has not been bound. " +
            $"Call SetNamedHttpClient(\"{_name}\", otherTestingServer.CreateClient()) after both test servers are initialized.");
}
