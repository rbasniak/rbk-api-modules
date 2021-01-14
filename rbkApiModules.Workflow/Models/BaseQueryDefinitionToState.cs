using System;

namespace rbkApiModules.Workflow
{
    public abstract class BaseQueryDefinitionToState<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup> 
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
        protected BaseQueryDefinitionToState()
        {

        }

        protected BaseQueryDefinitionToState(TQueryDefinition query, TState state)
        {
            State = state;
            Query = query;
        }

        public virtual Guid QueryId { get; private set; }
        public virtual TQueryDefinition Query { get; private set; }

        public virtual Guid StateId { get; private set; }
        public virtual TState State { get; private set; }

        public void SetKeys(TQueryDefinition query, TState state)
        {
            State = state;
            Query = query;
        }
    }
}
