namespace rbkApiModules.Commons.Testing;

internal sealed class HttpMockRule
{
    public required Type ClientType { get; init; }
    public required HttpMethod Method { get; init; }
    public string? Url { get; init; }
    public required Func<HttpResponseMessage> ResponseFactory { get; init; }

    public bool Matches(HttpRequestMessage request)
    {
        if (request.Method != Method)
        {
            return false;
        }

        if (Url is null)
        {
            return true;
        }

        var requestUrl = request.RequestUri?.ToString() ?? string.Empty;
        return string.Equals(requestUrl, Url, StringComparison.OrdinalIgnoreCase)
            || string.Equals(request.RequestUri?.PathAndQuery, Url, StringComparison.OrdinalIgnoreCase)
            || requestUrl.EndsWith(Url, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        var url = Url ?? "<any>";
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
        // Prefer exact URL matches over "any URL" rules; last registered wins within the same specificity.
        HttpMockRule? anyMatch = null;

        for (var i = _rules.Count - 1; i >= 0; i--)
        {
            var rule = _rules[i];
            if (rule.ClientType != clientType || !rule.Matches(request))
            {
                continue;
            }

            if (rule.Url is not null)
            {
                return rule;
            }

            anyMatch ??= rule;
        }

        return anyMatch;
    }

    public IEnumerable<HttpMockRule> ForClient(Type clientType) =>
        _rules.Where(r => r.ClientType == clientType);
}
