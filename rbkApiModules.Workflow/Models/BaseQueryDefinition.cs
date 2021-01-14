using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Workflow
{
    public abstract class BaseQueryDefinition<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup> : BaseEntity
        where TState : BaseState<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
        where TEvent : BaseEvent<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
        where TTransition : BaseTransition<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
        where TStateEntity : BaseStateEntity<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
        where TStateChangeEvent : BaseStateChangeEvent<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
        where TStateGroup : BaseStateGroup<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
        where TQueryDefinitionGroup: BaseQueryDefinitionGroup<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup> 
        where TQueryDefinition: BaseQueryDefinition<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup> 
        where TQueryDefinitionToState: BaseQueryDefinitionToState<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>, new()
        where TQueryDefinitionToGroup: BaseQueryDefinitionToGroup<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup>
    {
        private HashSet<TQueryDefinitionToState> _filteringStates;
        private HashSet<TQueryDefinitionToGroup> _groups;

        protected BaseQueryDefinition()
        {
            Claims = new string[0];
        }

        public BaseQueryDefinition(string name, string description, string menu, TState[] filteringStates, bool isActive, bool listOnlyUserItems, bool editableItems, string[] editableFields)
        {
            Claims = new string[0];

            _filteringStates = new HashSet<TQueryDefinitionToState>();
            _groups = new HashSet<TQueryDefinitionToGroup>();

            Update(name, description, filteringStates, isActive);
        }

        public virtual string Name { get; private set; }

        public virtual string Description { get; private set; }

        public virtual bool IsActive { get; private set; }

        public virtual string[] Claims { get; private set; }

        public virtual IEnumerable<TQueryDefinitionToState> FilteringStates => _filteringStates?.ToList();

        public virtual IEnumerable<TQueryDefinitionToGroup> Groups => _groups?.ToList();

        public virtual void Update(string name, string description, TState[] filteringStates, bool isActive)
        {
            // TODO: Fazer o delta para permitir atualizar os states de  filtragem
            if (_filteringStates.Count > 0) throw new ApplicationException("Não é possível atualizar os status de filtragem.");

            IsActive = isActive;
            Name = name;
            Description = description;

            foreach (var state in filteringStates)
            {
                var queryDefinition = new TQueryDefinitionToState();
                queryDefinition.SetKeys((TQueryDefinition)this, state);
                _filteringStates.Add(queryDefinition);
            }
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
