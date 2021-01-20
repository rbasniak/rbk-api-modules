using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Workflow
{
    public abstract class BaseStateEntity<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup> : BaseEntity
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
        protected HashSet<TStateChangeEvent> _events;

        protected BaseStateEntity()
        {

        }

        protected BaseStateEntity(TState state)
        {
            _events = new HashSet<TStateChangeEvent>();

            State = state;
        }

        public virtual Guid StateId { get; protected set; }
        public virtual TState State { get; protected set; }

        public virtual IEnumerable<TStateChangeEvent> Events => _events?.ToList();

        public virtual void NextStatus(TEvent trigger, string user, DbContext context)
        {
            if (State == null)
            {
                context.Entry(this).Reference(x => x.State).Load();
            }

            if (State.Transitions == null)
            {
                context.Entry(State).Collection(x => x.Transitions).Load();
            }

            foreach (var child in State.Transitions)
            {
                if (child.FromState == null) context.Entry(child).Reference(x => x.FromState).Load();
                if (child.ToState == null) context.Entry(child).Reference(x => x.ToState).Load();
                if (child.Event == null) context.Entry(child).Reference(x => x.Event).Load();
            }

            TTransition transition = null;

            var transitionHistory = String.Empty;

            transition = State.Transitions.FirstOrDefault(x => x.Event.Id == trigger.Id);

            if (transition != null)
            {
                State = transition.ToState;
                transitionHistory = transition.History;

                ExecuteDomainSpecificActions(user, transition);
            }
            else
            {
                throw new SafeException("Mudança de status inválida para a entidade selecionada");
            }

            var @event = CreateStateChangeEvent(user, transition);
            _events.Add(@event);
        }

        protected virtual void ExecuteDomainSpecificActions(string user, TTransition transition)
        {

        }

        protected abstract TStateChangeEvent CreateStateChangeEvent(string user, TTransition transition); 
    }
}
