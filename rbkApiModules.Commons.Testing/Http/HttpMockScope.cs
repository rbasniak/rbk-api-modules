namespace rbkApiModules.Commons.Testing;

/// <summary>
/// Isolates outbound HTTP mock rules to the current async flow via <see cref="AsyncLocal{T}"/>.
/// Shared <c>PerClass</c> testing servers keep one physical handler per client; rules are resolved from this scope.
/// </summary>
public sealed class HttpMockScope : IDisposable
{
    private static readonly AsyncLocal<HttpMockScope?> CurrentScope = new();

    private readonly HttpMockScope? _previous;
    private readonly HttpMockRuleBag _rules = new();
    private bool _disposed;

    private HttpMockScope()
    {
        _previous = CurrentScope.Value;
        CurrentScope.Value = this;
    }

    internal HttpMockRuleBag Rules => _rules;

    internal static HttpMockRuleBag? CurrentRules => CurrentScope.Value?._rules;

    public static HttpMockScope Begin() => new();

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        CurrentScope.Value = _previous;
    }
}
