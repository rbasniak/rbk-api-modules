using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public abstract class BaseGetQueryDefinitionResultsCommand: IRequest<QueryResponse>
    {
        public Guid[] QueryIds { get; set; }
    }

    public abstract class BaseGetQueryDefinitionResultsValidator<TQueryDefinition>: AbstractValidator<BaseGetQueryDefinitionResultsCommand> where TQueryDefinition: BaseEntity
    {
        public BaseGetQueryDefinitionResultsValidator(DbContext context)
        {
            RuleFor(x => x.QueryIds)
                .NotNull().WithMessage("O campo 'Queries' é obrigatório")
                .DependentRules(() => {
                    RuleForEach(x => x.QueryIds)
                        .MustExistInDatabase<BaseGetQueryDefinitionResultsCommand, TQueryDefinition>(context);
                });
        }
    }

    public abstract class BaseGetQueryDefinitionResultsHandler<TCommand, TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TStateEntityDto>
        : BaseQueryHandler<TCommand, DbContext>
        where TCommand : BaseGetQueryDefinitionResultsCommand
            where TState : BaseState<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
            where TEvent : BaseEvent<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
            where TTransition : BaseTransition<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
            where TStateEntity : BaseStateEntity<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
            where TStateChangeEvent : BaseStateChangeEvent<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
            where TStateGroup : BaseStateGroup<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
            where TQueryDefinitionGroup : BaseQueryDefinitionGroup<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
            where TQueryDefinition : BaseQueryDefinition<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
            where TQueryDefinitionToState : BaseQueryDefinitionToState<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>, new()
            where TQueryDefinitionToGroup : BaseQueryDefinitionToGroup<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
    {
        protected readonly IMapper _mapper;

        public BaseGetQueryDefinitionResultsHandler(DbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        protected override async Task<object> ExecuteAsync(TCommand request)
        {
            var queries = await LoadQueries(request);

            var results = new List<QueryDefinitionResults<TStateEntityDto>>();

            foreach (var query in queries)
            {
                if (!query.IsActive)
                {
                    continue;
                }

                var entities = await ApplyDomainFiltering(GetEntitiesQuery());

                results.Add(new QueryDefinitionResults<TStateEntityDto>(_mapper.Map<SimpleNamedEntity>(query), _mapper.Map<TStateEntityDto[]>(entities)));
            }

            return results;
        }

        /// <summary>
        /// Executes the query for the result entities. If you need to apply any extra filtering, override this method.
        /// </summary>
        protected virtual async Task<List<TStateEntity>> ApplyDomainFiltering(IIncludableQueryable<TStateEntity, TState> query)
        {
            return await query.ToListAsync();
        }

        /// <summary>
        /// Builds the query for the result entities. By default it only includes the State 
        /// navigation property. If you need to add more Includes, override this method
        /// </summary>
        protected virtual IIncludableQueryable<TStateEntity, TState> GetEntitiesQuery()
        {
            return _context.Set<TStateEntity>().Include(x => x.State);
        } 

        /// <summary>
        /// Returns an array of all queries received in the Command. If you need to add
        /// more Includes in the QueryDefinition object, override this method
        /// </summary>
        protected virtual Task<TQueryDefinition[]> LoadQueries(TCommand request)
        {
            var queries = request.QueryIds
                .Select(queryId => _context.Set<TQueryDefinition>()
                    .Include(x => x.FilteringStates)
                        .ThenInclude(x => x.State)
                    .Single(x => x.Id == queryId)
                );

            return Task.FromResult(queries.ToArray());
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
}
