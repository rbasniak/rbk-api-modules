using AutoMapper;
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
        public List<States.Details> States { get; set; }
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

    public class StatesMappings : Profile
    {
        public StatesMappings()
        {
            CreateMap<StateGroup, StateGroupDetails>();

            CreateMap<State, States.Details>();

            CreateMap<State, States.Simple>();

            CreateMap<Event, EventDetails>()
                .ForMember(dto => dto.Claims, map => map.MapFrom(entity => entity.Claims.Select(x => x.Claim)));

            CreateMap<Transition, TransitionDetails>()
                .ForMember(dto => dto.FromState, map => map.MapFrom(entity => entity.FromState))
                .ForMember(dto => dto.Event, map => map.MapFrom(entity => entity.Event))
                .ForMember(dto => dto.ToState, map => map.MapFrom(entity => entity.ToState));
        }
    }
}
