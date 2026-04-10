namespace rbkApiModules.Commons.Relational;

/// <summary>
/// Singleton implementation of <see cref="IDeferredSeedRunnerHealth"/>. Updated by <see cref="DeferredSeedRunnerHostedService"/>
/// so callers can determine when deferred seeds have run to completion or failed.
/// </summary>
public sealed class DeferredSeedRunnerHealth : IDeferredSeedRunnerHealth
{
    private readonly object _lock = new();
    private DeferredSeedRunnerStatus _status = DeferredSeedRunnerStatus.Pending;
    private string? _failureMessage;
    private string? _failedStepId;
    private List<string> _completedStepIds = new();

    /// <inheritdoc />
    public DeferredSeedRunnerStatus Status
    {
        get { lock (_lock) return _status; }
    }

    /// <inheritdoc />
    public bool IsCompleted => Status == DeferredSeedRunnerStatus.Completed;

    /// <inheritdoc />
    public bool IsFailed => Status == DeferredSeedRunnerStatus.Failed;

    /// <inheritdoc />
    public string? FailureMessage
    {
        get { lock (_lock) return _failureMessage; }
    }

    /// <inheritdoc />
    public string? FailedStepId
    {
        get { lock (_lock) return _failedStepId; }
    }

    /// <inheritdoc />
    public IReadOnlyList<string> CompletedStepIds
    {
        get { lock (_lock) return _completedStepIds.ToList(); }
    }

    /// <summary>
    /// Called by the runner when it starts. Not for external use.
    /// </summary>
    internal void RecordRunning()
    {
        lock (_lock)
        {
            _status = DeferredSeedRunnerStatus.Running;
            _completedStepIds = new List<string>();
            _failureMessage = null;
            _failedStepId = null;
        }
    }

    /// <summary>
    /// Called by the runner when a step completes successfully. Not for external use.
    /// </summary>
    internal void RecordStepCompleted(string stepId)
    {
        lock (_lock)
        {
            _completedStepIds.Add(stepId);
        }
    }

    /// <summary>
    /// Called by the runner when all steps finish successfully (or no steps ran). Not for external use.
    /// </summary>
    internal void RecordCompleted()
    {
        lock (_lock)
        {
            _status = DeferredSeedRunnerStatus.Completed;
            _failureMessage = null;
            _failedStepId = null;
        }
    }

    /// <summary>
    /// Called by the runner when a step fails. Not for external use.
    /// </summary>
    internal void RecordFailed(string? stepId, string failureMessage)
    {
        lock (_lock)
        {
            _status = DeferredSeedRunnerStatus.Failed;
            _failedStepId = stepId;
            _failureMessage = failureMessage ?? "Deferred seed failed.";
        }
    }
}
