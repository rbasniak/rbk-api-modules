using System.Collections.Concurrent;
using System.Reflection;

namespace rbkApiModules.Commons.Core;

public static class EventEnvelopeFactory
{
    private static readonly ConcurrentDictionary<Type, EventNameAttribute> Cache = new();

    public static EventEnvelope<TEvent> Wrap<TEvent>(TEvent @event, string tenantId, string username, string? correlationId = null, string? causationId = null)
    {
        if (@event is null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        if (tenantId is null)
        {
            throw new ArgumentNullException(nameof(tenantId));
        }

        if (username is null)
        {
            throw new ArgumentNullException(nameof(username));
        }

        var attr = GetEventNameAttribute(@event.GetType());

        return new EventEnvelope<TEvent>
        {
            EventId = Guid.NewGuid(),
            Name = attr.Name,
            Version = attr.Version,
            OccurredUtc = DateTime.UtcNow,
            TenantId = tenantId,
            Username = username,
            CorrelationId = correlationId,
            CausationId = causationId,
            Event = @event
        };
    }

    private static EventNameAttribute GetEventNameAttribute(Type t)
    {
        return Cache.GetOrAdd(t, static type =>
        {
            var attribute = type.GetCustomAttribute<EventNameAttribute>(inherit: false);

            if (attribute is null)
            {
                throw new InvalidOperationException($"Missing [EventName] on {type.FullName}");
            }

            return attribute;
        });
    }
} 