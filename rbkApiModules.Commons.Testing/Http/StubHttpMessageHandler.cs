namespace rbkApiModules.Commons.Testing;

internal sealed class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Type _clientType;
    private readonly Func<HttpMockRuleBag?> _getBackgroundRules;

    public StubHttpMessageHandler(Type clientType, Func<HttpMockRuleBag?> getBackgroundRules)
    {
        _clientType = clientType;
        _getBackgroundRules = getBackgroundRules;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestScopeRules = HttpMockScope.CurrentRules;
        if (requestScopeRules is not null)
        {
            return Respond(requestScopeRules, request);
        }

        var backgroundRules = _getBackgroundRules();
        if (backgroundRules is not null)
        {
            return Respond(backgroundRules, request);
        }

        throw new InvalidOperationException(
            $"No {nameof(HttpMockScope)} or {nameof(BackgroundHttpMockScope)} is active for {_clientType.Name}. " +
            "Use TestingServer.HttpMockScope() for in-request HTTP, or BackgroundHttpMocks() for background services.");
    }

    private Task<HttpResponseMessage> Respond(HttpMockRuleBag rules, HttpRequestMessage request)
    {
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
