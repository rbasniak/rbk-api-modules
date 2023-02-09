using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace Demo2.Domain.Events.Infrastructure;

public interface IEventStore
{
    Task SaveAsync(Guid aggregateId, int originatingVersion, IReadOnlyCollection<IDomainEvent> events, string aggregateName = "Aggregate Name");
    Task SaveAsync(Guid aggregateId, int originatingVersion, IDomainEvent @event, string aggregateName = "Aggregate Name");

    Task<IReadOnlyCollection<IDomainEvent>> LoadAsync(Guid aggregateRootId);
}

public class EventStoreRepository : IEventStore
{
    private string EventStoreTableName = "EventStore";

    private static string EventStoreListOfColumnsInsert = "[Id], [CreatedAt], [Version], [Name], [AggregateId], [Data], [Aggregate]";

    private static readonly string EventStoreListOfColumnsSelect = $"{EventStoreListOfColumnsInsert},[Sequence]";

    private readonly ISqlConnectionFactory _connectionFactory;

    public EventStoreRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyCollection<IDomainEvent>> LoadAsync(Guid aggregateRootId)
    {
        if (aggregateRootId == Guid.Empty) throw new AggregateRootNotProvidedException("AggregateRootId cannot be null");

        var query = new StringBuilder($@"SELECT {EventStoreListOfColumnsSelect} FROM {EventStoreTableName}");
        query.Append(" WHERE [AggregateId] = @AggregateId ");
        query.Append(" ORDER BY [Version] ASC;");

        using (var connection = _connectionFactory.SqlConnection())
        {
            var events = (await connection.QueryAsync<EventStoreDao>(query.ToString(), new { AggregateId = aggregateRootId.ToString() })).ToList();
            
            var domainEvents = events.Select(TransformEvent).Where(x => x != null).ToList().AsReadOnly();

            return domainEvents;
        }
    }

    private IDomainEvent TransformEvent(EventStoreDao eventSelected)
    {
        var o = JsonSerializer.Deserialize<DomainEvent>(eventSelected.Data);
        var evt = o as IDomainEvent;

        return evt;
    }


    public async Task SaveAsync(Guid aggregateId, int originatingVersion, IReadOnlyCollection<IDomainEvent> events, string aggregateName = "Aggregate Name")
    {
        if (events.Count == 0) return;

        var query =
            $@"INSERT INTO {EventStoreTableName} ({EventStoreListOfColumnsInsert})
                    VALUES (@Id,@CreatedAt,@Version,@Name,@AggregateId,@Data,@Aggregate);";

        var listOfEvents = events.Select(ev => new
        {
            Aggregate = aggregateName,
            ev.CreatedAt,
            Data = JsonSerializer.Serialize(ev),
            Id = Guid.NewGuid(),
            ev.GetType().Name,
            AggregateId = aggregateId.ToString(),
            Version = ++originatingVersion
        });

        using var connection = _connectionFactory.SqlConnection();
        await connection.ExecuteAsync(query, listOfEvents);
    }

    public async Task SaveAsync(Guid aggregateId, int originatingVersion, IDomainEvent @event, string aggregateName = "Aggregate Name")
    {
        if (@event == null) throw new InvalidOperationException("Event must be provided");

        await SaveAsync(aggregateId, originatingVersion, new List<IDomainEvent> { @event }, aggregateName);
    }
}