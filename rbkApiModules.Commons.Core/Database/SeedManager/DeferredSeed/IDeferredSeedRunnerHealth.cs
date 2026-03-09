namespace rbkApiModules.Commons.Relational;

/// <summary>
/// Exposes the status of the deferred seed runner. Injected as a singleton so callers can determine
/// whether all deferred seeds completed successfully or if a failure occurred.
/// </summary>
public interface IDeferredSeedRunnerHealth
{
    /// <summary>
    /// Current status of the deferred seed run.
    /// </summary>
    DeferredSeedRunnerStatus Status { get; }

    /// <summary>
    /// True when the runner has finished and all steps completed successfully (or no steps were run).
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// True when the runner finished with a failure.
    /// </summary>
    bool IsFailed { get; }

    /// <summary>
    /// When <see cref="IsFailed"/> is true, the exception or error message.
    /// </summary>
    string? FailureMessage { get; }

    /// <summary>
    /// When <see cref="IsFailed"/> is true, the id of the step that failed, if known.
    /// </summary>
    string? FailedStepId { get; }

    /// <summary>
    /// Ids of steps that completed successfully in this run (may be empty if no steps ran or runner failed before completing any).
    /// </summary>
    IReadOnlyList<string> CompletedStepIds { get; }
}

/// <summary>
/// Status of the deferred seed runner.
/// </summary>
public enum DeferredSeedRunnerStatus
{
    /// <summary>Runner has not started yet.</summary>
    Pending,

    /// <summary>Runner is currently executing steps.</summary>
    Running,

    /// <summary>Runner finished and all applicable steps completed successfully.</summary>
    Completed,

    /// <summary>Runner finished with a failure.</summary>
    Failed
}
