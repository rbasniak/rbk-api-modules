using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Workflow
{
    public abstract class BaseStateChangeEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup> : BaseEntity 
        where TState : BaseState<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TEvent : BaseEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TTransition : BaseTransition<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateEntity : BaseStateEntity<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TClaimToEvent : BaseClaimToEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateChangeEvent : BaseStateChangeEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateGroup : BaseStateGroup<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
    {
        protected BaseStateChangeEvent()
        {

        }

        public BaseStateChangeEvent(TStateEntity entity, string username, string statusName, string historyText, string notes)
        {
            Entity = entity;
            Username = username;
            Date = DateTime.Now;
            StatusHistory = historyText;
            StatusName = statusName;
            Notes = notes;
        }

        /// <summary>
        /// Texto com o nome do state que originou esse evento, no momento em que ele aconteceu
        /// </summary>
        public virtual string StatusName { get; protected set; }

        /// <summary>
        /// Texto para ser exibido no histórico da solicitação
        /// </summary>
        public virtual string StatusHistory { get; protected set; }

        /// <summary>
        /// Chave do usário que provocou a mudança de estado
        /// </summary>
        public virtual string Username { get; protected set; }

        public virtual DateTime Date { get; protected set; }

        /// <summary>
        /// Solicitação de mudança à qual este evento pertence
        /// </summary>
        public virtual Guid EntityId { get; protected set; }
        public virtual TStateEntity Entity { get; protected set; }

        public virtual string Notes { get; protected set; }

        public override string ToString()
        {
            return $"{Date.ToString()} - {StatusName}";
        }
    }
}
