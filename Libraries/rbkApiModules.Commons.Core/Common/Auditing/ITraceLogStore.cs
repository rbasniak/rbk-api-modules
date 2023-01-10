namespace rbkApiModules.Commons.Core.Auditing;

public interface ITraceLogStore
{
    Task Add(params TraceLog[] data);
}