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

    public class StatesMappings<TStateGroup, TSTate, TEvent, TTransition> : Profile
    {
        public StatesMappings()
        {
            CreateMap<TStateGroup, StateGroupDetails>();

            CreateMap<TSTate, States.Details>();

            CreateMap<TSTate, States.Simple>();

            CreateMap<TEvent, EventDetails>()
                .ForMember(dto => dto.Claims, map => map.MapFrom(entity => entity.Claims.Select(x => x.Claim)));

            CreateMap<TTransition, TransitionDetails>();
        }
    }

    public interface IClaims
    {
        IEnumerable<TClaimToEvent> Claims { get; }
    }
}
