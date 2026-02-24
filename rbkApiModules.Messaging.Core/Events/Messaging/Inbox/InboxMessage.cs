namespace rbkApiModules.Commons.Core;

public class InboxMessage
{
    public required Guid EventId { get; init; }
    public required string HandlerName { get; init; } = default!;
    public required DateTime ReceivedUtc { get; init; }
    public required DateTime? ProcessedUtc { get; init; }
    public required int Attempts { get; init; } = 0; 
} 