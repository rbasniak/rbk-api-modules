namespace rbkApiModules.Commons.Core.Auditing;

public class TraceLog
{
    protected TraceLog()
    {

    }

    public TraceLog(Guid commandId, string commandName, string payload, string username, Guid aggregateId, Guid entityId)
    {
        CommandId = commandId;
        CommandName = commandName;
        Username = username;
        Payload = payload;
        AggregateId = aggregateId;
        EntityId = entityId;
        Timestamp = DateTime.UtcNow;

    }

    public Guid Id { get; protected set; }
    public Guid CommandId { get; protected set; }
    public string CommandName { get; protected set; }
    public string Payload { get; protected set; }
    public string Username { get; protected set; }
    public Guid AggregateId { get; protected set; }
    public Guid EntityId { get; protected set; }
    public DateTime Timestamp { get; protected set; }
}