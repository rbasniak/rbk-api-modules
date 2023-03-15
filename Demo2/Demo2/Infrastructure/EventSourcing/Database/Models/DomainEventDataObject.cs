using Demo2.Domain.Events.Infrastructure;
using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.Domain;

public class DomainEventDataObject
{
    protected DomainEventDataObject()
    {

    }

    public DomainEventDataObject(IDomainEvent @event)
    {
        Id = @event.EventId;
        AggregateId = @event.AggregateId;
        CreatedAt = @event.CreatedAt;
        Version = @event.Version;
        Type = @event.GetType().FullName;
        Data = JsonSerializer.Serialize((object)@event);
    }

    public Guid Id { get; protected set; }
    public Guid AggregateId { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public int Version { get; protected set; }
    public string Type { get; protected set; }
    public string Data { get; protected set; }
}
