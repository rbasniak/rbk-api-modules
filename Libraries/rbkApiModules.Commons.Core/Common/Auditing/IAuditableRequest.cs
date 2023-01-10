namespace rbkApiModules.Commons.Core.Auditing;

public interface IAuditableResponse
{
    ChangedEntity[] AffectedEntities { get; }
}

public class ChangedEntity
{
    public ChangedEntity(Guid entityId, Guid aggregateId)
    {
        EntityId = entityId;
        AggregateId = aggregateId;
    }

    public Guid EntityId { get; }
    public Guid AggregateId { get; }
}
