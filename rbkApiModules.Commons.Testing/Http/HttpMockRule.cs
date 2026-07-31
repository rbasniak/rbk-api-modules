namespace rbkApiModules.Commons.Testing;

internal sealed class HttpMockRule
{
    public required Type ClientType { get; init; }
    public required HttpMethod Method { get; init; }
    public Func<string, bool>? UrlMatcher { get; init; }
    public required Func<HttpResponseMessage> ResponseFactory { get; init; }

    public bool Matches(HttpRequestMessage request)
    {
        if (request.Method != Method)
        {
            return false;
        }

        if (UrlMatcher is null)
        {
            return true;
        }

        var requestUrl = request.RequestUri?.ToString() ?? string.Empty;
        return UrlMatcher(requestUrl);
    }

    public override string ToString()
    {
        var url = UrlMatcher is null ? "<any>" : "<predicate>";
        return $"{Method} {url}";
    }
}

internal sealed class HttpMockRuleBag
{
    private readonly List<HttpMockRule> _rules = [];

    public void Add(HttpMockRule rule) => _rules.Add(rule);

    public IReadOnlyList<HttpMockRule> Rules => _rules;

    public HttpMockRule? FindMatch(Type clientType, HttpRequestMessage request)
    {
        for (var i = _rules.Count - 1; i >= 0; i--)
        {
            var rule = _rules[i];
            if (rule.ClientType == clientType && rule.Matches(request))
            {
                return rule;
            }
        }

        return null;
    }

    public IEnumerable<HttpMockRule> ForClient(Type clientType) =>
        _rules.Where(r => r.ClientType == clientType);
}
