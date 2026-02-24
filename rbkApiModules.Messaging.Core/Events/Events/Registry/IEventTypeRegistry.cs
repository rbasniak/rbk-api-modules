namespace rbkApiModules.Commons.Core;

public interface IEventTypeRegistry
{
    bool TryResolve(string name, short version, out Type type);
} 