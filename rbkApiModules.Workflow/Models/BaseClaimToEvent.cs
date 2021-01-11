using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Workflow
{
    public abstract class BaseClaimToEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TState : BaseState<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TEvent : BaseEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TTransition : BaseTransition<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateEntity : BaseStateEntity<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TClaimToEvent : BaseClaimToEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateChangeEvent : BaseStateChangeEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateGroup : BaseStateGroup<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
    {
        protected BaseClaimToEvent()
        {

        }

        public BaseClaimToEvent(TEvent @event, string claim)
        {
            Claim = claim;
            Event = @event;
        }

        public virtual string Claim { get; protected set; }

        public virtual Guid EventId { get; protected set; }
        public virtual TEvent Event { get; protected set; }

        public override string ToString()
        {
            return Claim + " - " + Event?.Name;
        }
    }
}
