using AutoMapper;
using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Workflow.Core;

public class GetQueryDefinitionResults
{
    public class Command : IRequest<QueryResponse>
    {
        public Guid[] QueryIds { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(IStatesService statesService)
        {
            RuleFor(x => x.QueryIds)
                .NotNull().WithMessage("O campo 'Queries' é obrigatório")
                .DependentRules(() => {
                    RuleForEach(x => x.QueryIds)
                        .MustAsync(async (command, queryId, cancellation) => await statesService.FindQueryDefinition(queryId, cancellation) != null).WithMessage("Could not found query definition");
                });
        }
    }

    public abstract class BaseGetQueryDefinitionResultsHandler : IRequestHandler<Command, QueryResponse>
    {
        protected readonly IStatesService _statesService;
        protected readonly IMapper _mapper;

        public BaseGetQueryDefinitionResultsHandler(IStatesService statesService, IMapper mapper)
        {
            _statesService = statesService;
            _mapper = mapper;
        }

        public async Task<QueryResponse> Handle(Command request, CancellationToken cancellation)
        {
            var queries = await _statesService.GetQueries(request.QueryIds, cancellation);

            var results = new List<QueryDefinitionResults<StateEntityDetails>>();

            foreach (var query in queries)
            {
                if (!query.IsActive)
                {
                    continue;
                }

                throw new NotImplementedException("Impolemntar a execucao da query");

                BaseEntity[] entities = null;

                results.Add(new QueryDefinitionResults<StateEntityDetails>(_mapper.Map<SimpleNamedEntity>(query), _mapper.Map<StateEntityDetails[]>(entities)));
            }

            return QueryResponse.Success(results);
        }
    }
}

public class QueryDefinitionResults<T>
{
    public QueryDefinitionResults(SimpleNamedEntity queryDefinition, T[] results)
    {
        QueryDefinition = queryDefinition;
        Results = results;
    }

    public SimpleNamedEntity QueryDefinition { get; set; }
    public T[] Results { get; set; }
}
