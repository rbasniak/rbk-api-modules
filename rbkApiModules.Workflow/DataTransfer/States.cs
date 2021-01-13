﻿using AutoMapper;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public class StateGroupDetails: BaseDataTransferObject
    {
        public string Name { get; set; }
        public States.Details[] States { get; set; }
    }

    public class States
    {
        public class Details : BaseDataTransferObject
        {
            public string Name { get; set; }
            public string SystemId { get; set; }
            public bool IsProtected { get; set; }
            public SimpleNamedEntity Group { get; set; }
            public List<TransitionDetails> Transitions { get; set; }
        }

        public class Simple : BaseDataTransferObject
        {
            public string Name { get; set; }
            public string Color { get; set; }
        }
    }

    public class TransitionDetails
    {
        public SimpleNamedEntity FromState { get; set; }
        public EventDetails Event { get; set; }
        public SimpleNamedEntity ToState { get; set; }
        public bool IsProtected { get; set; }
    }

    public class EventDetails
    {
        public string Name { get; set; }
        public string SystemId { get; set; }
        public string[] Claims { get; set; }
    }

    public class StatesMappings<TState, TEvent, TTransition, TStateEntity, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup> : Profile
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
        public StatesMappings()
        {
            CreateMap<TQueryDefinition, SimpleNamedEntity>();

            CreateMap<TStateGroup, SimpleNamedEntity>();

            CreateMap<TStateGroup, StateGroupDetails>();

            CreateMap<TState, States.Details>();

            CreateMap<TState, States.Simple>();
            
            CreateMap<TState, SimpleNamedEntity>();

            CreateMap<TEvent, EventDetails>();

            CreateMap<TTransition, TransitionDetails>();
        }
    } 
}
