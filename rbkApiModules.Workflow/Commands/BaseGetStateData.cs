using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public abstract class BaseGetStateDataCommand: IRequest<QueryResponse>
    {
    }

    public abstract class BaseGetStateDataValidator<T>: AbstractValidator<BaseGetStateDataCommand> where T: BaseEntity
    {
    }

    public abstract class BaseGetStateDataHandler<TCommand, TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TStateGroupDto>
        : BaseQueryHandler<TCommand, DbContext>
        where TCommand : BaseGetStateDataCommand
        where TState : BaseState<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TEvent : BaseEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TTransition : BaseTransition<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateEntity : BaseStateEntity<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TClaimToEvent : BaseClaimToEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateChangeEvent : BaseStateChangeEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateGroup : BaseStateGroup<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
    {
        protected readonly IMapper _mapper;

        public BaseGetStateDataHandler(DbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        protected override async Task<object> ExecuteAsync(TCommand request)
        {
            var result = Map(await LoadData(request));

            return result;
        }

        protected virtual async Task<TStateGroup[]> LoadData(TCommand request)
        {
            var transitions = await _context.Set<TTransition>().ToListAsync();
            var events = await _context.Set<TEvent>().ToListAsync();
            var states = await _context.Set<TState>().ToListAsync();
            var claims = await _context.Set<TClaimToEvent>().ToListAsync();
            var groups = await _context.Set<TStateGroup>().ToArrayAsync();

            return groups;
        }

        protected virtual TStateGroupDto[] Map(TStateGroup[] data)
        {
            return _mapper.Map<TStateGroupDto[]>(data);
        }
    }
}
