using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rbkApiModules.Workflow
{
    public abstract class BaseState<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup> : BaseEntity
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
        protected HashSet<TTransition> _usedBy;
        protected HashSet<TStateEntity> _items;

        protected BaseState()
        {

        }

        /// <summary>
        /// Método só deve ser utilizado para testes unitários.
        /// </summary>
        protected BaseState(Guid id, string name, TStateGroup group, string systemId, string color, bool isActive): 
            this(group, name, systemId, color, isActive)
        {
            Id = id;
        }

        private BaseState(string name, string systemId, string color, bool isActive)
        {
            _items = new HashSet<TStateEntity>();
            _transitions = new HashSet<TTransition>();
            _usedBy = new HashSet<TTransition>();

            Name = name;
            SystemId = systemId;
            Color = color;
            IsActive = isActive;
        }

        protected BaseState(Guid groupId, string name, string systemId, string color, bool isActive)
            : this(name, systemId, color, isActive)
        {
            GroupId = groupId;
        }

        protected BaseState(TStateGroup group, string name, string systemId, string color, bool isActive)
            : this(name, systemId, color, isActive)
        {
            Group = group;
        }

        public virtual string Name { get; protected set; }

        public virtual bool IsActive { get; protected set; }

        /// <summary>
        /// Id interno do state, para ser utilizado nas partes em que as transições são hardcoded
        /// </summary>
        public virtual string SystemId { get; protected set; }

        public virtual Guid GroupId { get; protected set; }
        public virtual TStateGroup Group { get; protected set; }

        /// <summary>
        /// Cor para o label no front
        /// </summary>
        public virtual string Color { get; protected set; }

        /// <summary>
        /// Flag que indica se o state pode ou não ser apagado
        /// </summary>
        public virtual bool IsProtected => !String.IsNullOrEmpty(SystemId);

        public virtual IEnumerable<TTransition> Transitions => _transitions?.ToList();
        public virtual IEnumerable<TTransition> UsedBy => _usedBy?.ToList();
        public virtual IEnumerable<TStateEntity> Items => _items?.ToList();

        public virtual void Update(string name, string color, bool isActive)
        {
            IsActive = isActive;
            Name = name;
            Color = color;
        }

        public override string ToString()
        {
            return $"{Name} [{Transitions.Count()} transitions]";
        }
    }
}
