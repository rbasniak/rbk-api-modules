using rbkApiModules.Commons.Core.Auditing;

namespace rbkApiModules.Commons.Core;

public class CommandResponse : BaseResponse
{
    public CommandResponse() : base()
    {
    }

    internal CommandResponse(object result) : base(result)
    {
    }

    public ChangedEntity[] AffectedEntities { get; }

    public static CommandResponse Success()
    {
        return CommandResponse.Success(null);
    }

    public static CommandResponse Success(object result)
    {
        var response = new CommandResponse(result);

        return response;
    }
}

public class AuditableCommandResponse : BaseResponse, IAuditableResponse
{
    private HashSet<ChangedEntity> _changedEntities;

    public AuditableCommandResponse() : base()
    {
        _changedEntities = new HashSet<ChangedEntity>();
    }

    internal AuditableCommandResponse(object result) : base(result)
    {
        _changedEntities = new HashSet<ChangedEntity>();
    }

    public ChangedEntity[] AffectedEntities => _changedEntities.ToArray();

    public static AuditableCommandResponse Success(object result, Guid entityId)
    {
        var response = new AuditableCommandResponse(result);

        response.AddAggregate(entityId);

        return response;
    }

    public static AuditableCommandResponse Success(object result, Guid entityId, Guid aggregateId)
    {
        var response = new AuditableCommandResponse(result);

        response.AddEntity(entityId, aggregateId);

        return response;
    }

    public static AuditableCommandResponse Success(object result, Guid[] entityIds, Guid aggregateId)
    {
        var response = new AuditableCommandResponse(result);

        foreach (var entityId in entityIds)
        {
            response.AddEntity(entityId, aggregateId);
        }

        return response;
    }

    public static AuditableCommandResponse Success(object result, ChangedEntity[] data)
    {
        var response = new AuditableCommandResponse(result);

        response.SetTraceData(data);

        return response;
    }

    private void SetTraceData(ChangedEntity[] data)
    {
        _changedEntities = data.ToHashSet();
    }

    private void AddEntity(Guid entityId, Guid aggregateId)
    {
        _changedEntities.Add(new ChangedEntity(entityId, aggregateId));
    }

    private void AddAggregate(Guid entityId)
    {
        _changedEntities.Add(new ChangedEntity(entityId, entityId));
    }
}