using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rbkApiModules.Workflow
{
    public abstract class BaseEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup> : BaseEntity, IClaims
        where TState : BaseState<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TEvent : BaseEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TTransition : BaseTransition<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateEntity : BaseStateEntity<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TClaimToEvent : BaseClaimToEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateChangeEvent : BaseStateChangeEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateGroup : BaseStateGroup<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
    {
        protected HashSet<TClaimToEvent> _claims;
        protected HashSet<TTransition> _transitions;

        protected BaseEvent()
        {

        }

        public BaseEvent(string name, string systemId, bool isActive)
        {
            _claims = new HashSet<TClaimToEvent>();
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

        public virtual bool IsProtected => !String.IsNullOrEmpty(SystemId);

        public virtual IEnumerable<TTransition> Transitions => _transitions?.ToList();

        public virtual IEnumerable<TClaimToEvent> Claims => _claims?.ToList();

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
