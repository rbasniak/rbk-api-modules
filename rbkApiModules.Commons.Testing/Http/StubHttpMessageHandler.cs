namespace rbkApiModules.Commons.Testing;

internal sealed class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Type _clientType;

    public StubHttpMessageHandler(Type clientType)
    {
        _clientType = clientType;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var rules = HttpMockScope.CurrentRules
            ?? throw new InvalidOperationException(
                $"No {nameof(HttpMockScope)} is active for {_clientType.Name}. " +
                "Wrap the arrange/act section with: using var _ = TestingServer.HttpMockScope();");

        var match = rules.FindMatch(_clientType, request);
        if (match is null)
        {
            var registered = rules.ForClient(_clientType).Select(r => r.ToString()).ToArray();
            var registeredText = registered.Length == 0
                ? "(none)"
                : string.Join(", ", registered);

            throw new InvalidOperationException(
                $"No HTTP mock rule matched {_clientType.Name} {request.Method} {request.RequestUri}. " +
                $"Registered rules: {registeredText}");
        }

        return Task.FromResult(match.ResponseFactory());
    }
}
