﻿using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public abstract class BaseStateEntity<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup> : BaseEntity
        where TState : BaseState<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TEvent : BaseEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TTransition : BaseTransition<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateEntity : BaseStateEntity<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TClaimToEvent : BaseClaimToEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateChangeEvent : BaseStateChangeEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        where TStateGroup : BaseStateGroup<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
    {
        protected HashSet<TStateChangeEvent> _events;

        protected BaseStateEntity()
        {

        }

        public BaseStateEntity(TState state)
        {
            _events = new HashSet<TStateChangeEvent>();

            State = state;
        }

        public virtual Guid StateId { get; protected set; }
        public virtual TState State { get; protected set; }

        public virtual IEnumerable<TStateChangeEvent> Events => _events?.ToList();
    }
}
