namespace rbkApiModules.Workflow.Core;

public interface IStatesCacheService
{
    bool IsInitialized { get; }
}

public interface IEventsCacheService
{
    bool IsInitialized { get; }
}

