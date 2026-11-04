namespace rbkApiModules.Commons.Testing;

/// <summary>
/// Mock rules for outbound HTTP from background work (timers, hosted services) that does not flow
/// <see cref="HttpMockScope"/> <c>AsyncLocal</c>. One active scope per <see cref="RbkTestingServer{TProgram}"/> instance.
/// </summary>
public sealed class BackgroundHttpMockScope : IDisposable
{
    private readonly RbkTestingServerHolder _server;
    private readonly HttpMockRuleBag _rules = new();
    private bool _disposed;

    internal BackgroundHttpMockScope(RbkTestingServerHolder server)
    {
        _server = server;
        _server.AttachBackgroundScope(this);
    }

    internal HttpMockRuleBag Rules => _rules;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _server.DetachBackgroundScope(this);
    }

    internal sealed class RbkTestingServerHolder
    {
        private readonly object _gate = new();
        private BackgroundHttpMockScope? _active;

        internal void AttachBackgroundScope(BackgroundHttpMockScope scope)
        {
            lock (_gate)
            {
                if (_active is not null)
                {
                    throw new InvalidOperationException(
                        $"Another {nameof(BackgroundHttpMockScope)} is already active on this testing server. " +
                        "Dispose it before opening a new one.");
                }

                _active = scope;
            }
        }

        internal void DetachBackgroundScope(BackgroundHttpMockScope scope)
        {
            lock (_gate)
            {
                if (_active == scope)
                {
                    _active = null;
                }
            }
        }

        internal HttpMockRuleBag? ActiveRules
        {
            get
            {
                lock (_gate)
                {
                    return _active?._rules;
                }
            }
        }
    }
}
