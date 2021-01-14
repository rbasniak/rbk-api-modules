using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Workflow.Services;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public abstract class BaseGetStatesDotCodeCommand : IRequest<QueryResponse>
    {
        public Guid GroupId { get; set; }
    }

    public abstract class BaseGetStatesDotCodeValidator<T> : AbstractValidator<BaseGetStatesDotCodeCommand> where T : BaseEntity
    {
        public BaseGetStatesDotCodeValidator(DbContext context)
        {
            RuleFor(x => x.GroupId)
                .MustExistInDatabase<BaseGetStatesDotCodeCommand, T>(context);
        }
    }

    public abstract class BaseGetStatesDotCodeHandler<TCommand, TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
        : BaseQueryHandler<TCommand, DbContext>
        where TCommand : BaseGetStatesDotCodeCommand
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

        public BaseGetStatesDotCodeHandler(DbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        protected override async Task<object> ExecuteAsync(TCommand request)
        {
            var states = await _context.Set<TState>()
                .Include(x => x.Transitions).ThenInclude(x => x.Event)
                .Include(x => x.Transitions).ThenInclude(x => x.FromState)
                .Include(x => x.Transitions).ThenInclude(x => x.ToState)
                .Include(x => x.UsedBy).ThenInclude(x => x.Event)
                .Include(x => x.UsedBy).ThenInclude(x => x.FromState)
                .Include(x => x.UsedBy).ThenInclude(x => x.ToState)
                .ToListAsync();

            var code = DotCodeGenerator.GenerateCode<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>(states);

            return new DotCodeResponse()
            {
                Code = code
            };
        }
    }
}
