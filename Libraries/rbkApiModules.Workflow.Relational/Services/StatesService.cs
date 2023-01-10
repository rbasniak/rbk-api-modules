using MediatR;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Workflow.Core;

namespace rbkApiModules.Workflow.Relational;

public class StatesService : IStatesService
{
    private readonly DbContext _context;

    public StatesService(DbContext context)
    {
        _context = context;
    }

    public async Task<StateGroup> FindGroup(Guid id, CancellationToken cancellation = default)
    {
        return await _context.Set<StateGroup>().FindAsync(id, cancellation);
    }

    public async Task<QueryDefinition[]> FindQueryDefinition(Guid[] queryIds, CancellationToken cancellation = default)
    {
        var queries = queryIds
                .Select(queryId => _context.Set<QueryDefinition>()
                    .Include(x => x.FilteringStates)
                        .ThenInclude(x => x.State)
                    .Single(x => x.Id == queryId)
                ).ToArray();

        return queries;
    }

    public async Task<QueryDefinition> FindQueryDefinition(Guid id, CancellationToken cancellation = default)
    {
        return await _context.Set<QueryDefinition>()
                    .Include(x => x.FilteringStates)
                        .ThenInclude(x => x.State)
                    .SingleAsync(x => x.Id == id);
    }

    public async Task<StateChangeEvent[]> GetEntityHistory(Guid id, CancellationToken cancellation = default)
    {
        var results = await _context.Set<StateChangeEvent>().Where(x => x.EntityId == id).ToArrayAsync();

        return results;
    }

    public async Task<StateGroup[]> GetGroups(CancellationToken cancellation = default)
    {
        var transitions = await _context.Set<Transition>().ToListAsync();
        var events = await _context.Set<Event>().ToListAsync();
        var states = await _context.Set<State>().ToListAsync();
        var groups = await _context.Set<StateGroup>().ToArrayAsync();

        return groups;
    }

    public Task<QueryDefinition[]> GetQueries(Guid[] queryIds, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public async Task<State[]> GetStates(CancellationToken cancellationToken = default)
    {
        var states = await _context.Set<State>()
               .Include(x => x.Transitions).ThenInclude(x => x.Event)
               .Include(x => x.Transitions).ThenInclude(x => x.FromState)
               .Include(x => x.Transitions).ThenInclude(x => x.ToState)
               .Include(x => x.UsedBy).ThenInclude(x => x.Event)
               .Include(x => x.UsedBy).ThenInclude(x => x.FromState)
               .Include(x => x.UsedBy).ThenInclude(x => x.ToState)
               .ToArrayAsync();

        return states;
    }
}
