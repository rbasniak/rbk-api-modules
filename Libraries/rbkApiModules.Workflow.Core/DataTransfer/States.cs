using AutoMapper;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

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

public class StateChangedEventDetails
{
    public string Username { get; set; }
    public DateTime Date { get; set; }
    public string StatusHistory { get; set; }
    public string StatusName { get; set; }
    public string Notes { get; set; }
}

public class TransitionDetails: BaseDataTransferObject
{
    public SimpleNamedEntity FromState { get; set; }
    public EventDetails Event { get; set; }
    public SimpleNamedEntity ToState { get; set; }
    public bool IsProtected { get; set; }
}

public class EventDetails: BaseDataTransferObject
{
    public string Name { get; set; }
    public string SystemId { get; set; }
    public string[] Claims { get; set; }
}

public class StateEntityDetails: BaseDataTransferObject
{

}

public class StatesMappings : Profile
{
    public StatesMappings()
    {
        CreateMap<QueryDefinition, SimpleNamedEntity>();

        CreateMap<StateGroup, SimpleNamedEntity>();

        CreateMap<StateGroup, StateGroupDetails>();

        CreateMap<State, States.Details>();

        CreateMap<State, States.Simple>();
        
        CreateMap<State, SimpleNamedEntity>();

        CreateMap<Event, EventDetails>();

        CreateMap<Transition, TransitionDetails>();

        CreateMap<StateChangeEvent, StateChangedEventDetails>();
    }
} 
