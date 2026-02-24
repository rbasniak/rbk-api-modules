// TODO: DONE, REVIEWED

namespace rbkApiModules.Commons.Core;

public sealed class DomainEventDispatcherOptions
{
    private Func<IServiceProvider, MessagingDbContext>? _resolveSilentContext;
    private Func<IServiceProvider, MessagingDbContext>? _resolveDbContext;

    public int BatchSize { get; set; } = 50;
    public int PollIntervalMs { get; set; } = 1000;
    public int MaxAttempts { get; set; } = 5;
    public int ClaimDurationMin { get; set; } = 5;

    // Required: provide a way to resolve the application's DbContext
    public Func<IServiceProvider, MessagingDbContext> ResolveSilentDbContext 
    { 
        get
        {
            if (_resolveSilentContext == null)
            {
                throw new InvalidOperationException("ResolveSilentDbContext must be set before using OutboxOptions.");
            }
            return _resolveSilentContext;
        } 
        set
        {
            _resolveSilentContext = value;
        }
    }
    public Func<IServiceProvider, MessagingDbContext> ResolveDbContext
    {
        get
        {
            if (_resolveDbContext == null)
            {
                throw new InvalidOperationException("ResolveDbContext must be set before using OutboxOptions.");
            }

            return _resolveDbContext;
        }
        set
        {
            _resolveDbContext = value;
        }
    }
} 