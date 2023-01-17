namespace rbkApiModules.Workflow.Core;

public interface IStatesService
{
    Task<StateGroup> FindGroup(Guid id, CancellationToken cancellation = default);
    Task<QueryDefinition> FindQueryDefinition(Guid id, CancellationToken cancellation = default);
    Task<QueryDefinition[]> FindQueryDefinition(Guid[] queryIds, CancellationToken cancellation = default);
    Task<StateChangeEvent[]> GetEntityHistory(Guid id, CancellationToken cancellation = default);
    Task<StateGroup[]> GetGroups(CancellationToken cancellation = default);
    Task<QueryDefinition[]> GetQueries(Guid[] queryIds, CancellationToken cancellation = default);
    Task<State[]> GetStates(CancellationToken cancellation = default);
}
