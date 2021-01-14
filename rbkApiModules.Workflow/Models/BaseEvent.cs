using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Workflow
{
    public abstract class BaseEvent<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup> : BaseEntity
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
        protected HashSet<TTransition> _transitions;

        protected BaseEvent()
        {
            Claims = new string[0];
        }

        protected BaseEvent(string name, string systemId, bool isActive)
        {
            Claims = new string[0];

            _transitions = new HashSet<TTransition>();

            Name = name;
            SystemId = systemId;
            IsActive = isActive;
        }

        /// <summary>
        /// Construtor para ser usado apenas em testes unitários
        /// </summary>
        public BaseEvent(Guid id, string name, string systemId, bool isActive) : this(name, systemId, isActive)
        {
            Id = id;
        }

        public virtual string Name { get; protected set; }

        public virtual string SystemId { get; protected set; }

        public virtual bool IsActive { get; protected set; }

        public virtual string[] Claims { get; private set; }

        public virtual bool IsProtected => !String.IsNullOrEmpty(SystemId);

        public virtual IEnumerable<TTransition> Transitions => _transitions?.ToList();

        public virtual void Update(string name, bool isActive)
        {
            Name = name;
            IsActive = isActive;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
