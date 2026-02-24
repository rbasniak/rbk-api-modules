using System.Reflection;

namespace rbkApiModules.Commons.Core.Messaging;

public static class EventUtils
{
    public static string GetEventName(this Type eventType)
    {
        var attribute = eventType.GetCustomAttribute<EventNameAttribute>(inherit: false);

        if (attribute is null)
        {
            throw new InvalidOperationException($"Missing [EventName] on {eventType.FullName}");
        }

        return attribute.Name;
    }
}
