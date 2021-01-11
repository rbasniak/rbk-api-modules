using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Workflow
{
    public abstract class BaseTransition<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup> : BaseEntity
        where TState : BaseState<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TEvent : BaseEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TTransition : BaseTransition<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateEntity : BaseStateEntity<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TClaimToEvent : BaseClaimToEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateChangeEvent : BaseStateChangeEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateGroup : BaseStateGroup<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
    {
        protected BaseTransition()
        {

        }

        public BaseTransition(Guid fromStateId, Guid eventId, Guid toStateId, string history, bool isProtected, bool isActive)
        {
            FromStateId = fromStateId;
            ToStateId = toStateId;
            EventId = eventId;
            History = history;
            IsProtected = isProtected;
            IsActive = isActive;
        }

        public BaseTransition(TState fromState, TEvent @event, TState toState, string history, bool isProtected, bool isActive)
        {
            FromState = fromState;
            ToState = toState;
            Event = @event;
            History = history;
            IsProtected = isProtected;
            IsActive = isActive;
        }

        public virtual bool IsActive { get; protected set; }

        /// <summary>
        /// State ao qual essa transição pertence
        /// </summary>
        public virtual Guid FromStateId { get; protected set; }
        public virtual TState FromState { get; protected set; }

        /// <summary>
        /// State para o qual essa transição leva
        /// </summary>
        public virtual Guid ToStateId { get; protected set; }
        public virtual TState ToState { get; protected set; }

        /// <summary>
        /// Evento que dispara essa transição
        /// </summary>
        public virtual Guid EventId { get; protected set; }
        public virtual TEvent Event { get; protected set; }

        /// <summary>
        /// Flag que indica se é possível ou não apagar essa transição
        /// </summary>
        public virtual bool IsProtected { get; protected set; }

        /// <summary>
        /// Texto para ser exibido no histórico de operações da requisição
        /// </summary>
        public virtual string History { get; protected set; }

        public virtual void Update(TEvent @event, TState toState, string history)
        {
            Event = @event;
            ToState = toState;
            History = history;
        }

        public override string ToString()
        {
            return $"{FromState.Name} -> {Event.Name} -> {ToState.Name}";
        }
    }
}
