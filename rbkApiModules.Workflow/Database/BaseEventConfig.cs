using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public abstract class BaseEventConfig
    {
        protected void Configure<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>(EntityTypeBuilder<TEvent> entity)
            where TState : BaseState<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
            where TEvent : BaseEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
            where TTransition : BaseTransition<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
            where TStateEntity : BaseStateEntity<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
            where TClaimToEvent : BaseClaimToEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
            where TStateChangeEvent : BaseStateChangeEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
            where TStateGroup : BaseStateGroup<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup>
        {
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(1024);
        }
    }
}
