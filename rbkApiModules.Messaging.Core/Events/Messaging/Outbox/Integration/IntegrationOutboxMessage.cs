// TODO: DONE, REVIEWED

namespace rbkApiModules.Commons.Core;

/// <summary>
/// Represents an integration event stored in the integration outbox.
/// </summary>
public class IntegrationOutboxMessage : ITelemetryPropagationDataCarrier
{
    public required Guid Id { get; init; }
    public required string Name { get; init; } = string.Empty;
    public required short Version { get; init; }
    public required string TenantId { get; init; } = string.Empty;
    public required DateTime OccurredUtc { get; init; }
    public required string? CorrelationId { get; init; }
    public required string? CausationId { get; init; }
    public required string Payload { get; init; } = string.Empty;
    public required DateTime CreatedUtc { get; init; }
    public required string Username { get; init; } = string.Empty;
    public required string? TraceId { get; init; }
    public required string? ParentSpanId { get; init; }
    public required int? TraceFlags { get; init; }
    public required string? TraceState { get; init; }

    // Time when the message was published to the broker, so it was
    // processed in the outbox, but each consumer will later process it
    // with their own logic
    public DateTime? ProcessedUtc { get; set; }
    public short Attempts { get; private set; }
    public DateTime? DoNotProcessBeforeUtc { get; private set; }
    public DateTime? ClaimedUntil { get; internal set; }
    public Guid? ClaimedBy { get; internal set; }
    public bool IsPoisoned { get; private set; }

    internal void Backoff()
    {
        Attempts++;
        DoNotProcessBeforeUtc = DateTime.UtcNow.Add(ComputeBackoff(Attempts));
        ClaimedUntil = null;
        ClaimedBy = null;
    }

    internal void MarkAsProcessed()
    {
        ProcessedUtc = DateTime.UtcNow;
        ClaimedUntil = null;
        ClaimedBy = null;
        Attempts++;
    }

    internal void MarkAsPoisoned()
    {
        IsPoisoned = true;
        ClaimedUntil = null;
        ClaimedBy = null;
        Attempts++;
    }

    private TimeSpan ComputeBackoff(int attempts)
    {
        // Exponential backoff with a maximum of 300 seconds (5 minutes)
        var baseSeconds = Math.Min(300, (int)Math.Pow(2, Math.Min(10, attempts)));

        var jitter = Random.Shared.Next(0, 1000);

        var backoff = TimeSpan.FromSeconds(baseSeconds).Add(TimeSpan.FromMilliseconds(jitter));

        return backoff;
    }
}
