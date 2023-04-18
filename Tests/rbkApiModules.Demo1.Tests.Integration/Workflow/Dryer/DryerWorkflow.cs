using Stateless;

namespace rbkApiModules.Demo1.Tests.Integration.Workflow.Dryer;

public class DryerWorkflow
{
    public enum State { On, Off }

    public enum Trigger { TurnOn, TurnOff }

    public readonly StateMachine<State, Trigger> _machine;

    private readonly Dryer _dryer;

    public DryerWorkflow(Dryer drier)
    {
        _dryer = drier;

        _machine = new StateMachine<State, Trigger>(_dryer.State);

        _machine.OnTransitionCompleted(t => 
        {
            _dryer.State = t.Destination;

            _dryer.AddEvent(new DryerWorkflowEvent
            {
                Name = "GLOBAL_EVENT::OnTransitionCompleted",
                PreviousState = t.Source,
                NextState = t.Destination,
                Trigger = t.Trigger,
                Type = EventType.OnTransitionCompleted
            });
        });

        _machine.OnTransitioned(t =>
        {
            _dryer.AddEvent(new DryerWorkflowEvent
            {
                Name = "GLOBAL_EVENT::OnTransitioned",
                PreviousState = t.Source,
                NextState = t.Destination,
                Trigger = t.Trigger,
                Type = EventType.OnTransitioned
            });
        });

        _machine.Configure(State.Off)
            .PermitIf(Trigger.TurnOn, State.On, 
                new GuardDefinition("Electricity price guard", args => {
                    var price = (double)args[0];
                    var result = price < 2;
                    
                    _dryer.AddEvent(new DryerWorkflowEvent
                    {
                        Name = "GUARD::" + (result ? "Passed" : "Blocked"),
                        PreviousState = State.Off,
                        NextState = State.On,
                        Trigger = Trigger.TurnOn,
                        Type = EventType.TransitionGuard
                    });

                    return result;
                }))
            .OnEntry(x => _dryer.AddEvent(new DryerWorkflowEvent
            {
                Name = "OFF:OnEntry",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnEntry
            }), "OFF:OnEntry")
            .OnExit(x => _dryer.AddEvent(new DryerWorkflowEvent
            {
                Name = "OFF:OnExit",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnExit
            }), "OFF:OnExit");

        _machine.Configure(State.On)
            .Permit(Trigger.TurnOff, State.Off)
            .OnEntry(x => _dryer.AddEvent(new DryerWorkflowEvent
            {
                Name = "ON:OnEntry",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnEntry
            }), "ON:OnEntry")
            .OnExit(x => _dryer.AddEvent(new DryerWorkflowEvent
            {
                Name = "ON:OnExit",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnExit
            }), "ON:OnExit");
    }  

    public void TurnOn(double price)
    {
        _machine.Fire(Trigger.TurnOn, price);
    }

    public void TurnOff()
    {
        _machine.Fire(Trigger.TurnOff);
    }
}

public class Dryer
{
    public Dryer()
    {
        State = DryerWorkflow.State.Off;
        Events = new List<DryerWorkflowEvent>();
    }

    public DryerWorkflow.State State { get; set; }
    public List<DryerWorkflowEvent> Events { get; set; }

    internal void AddEvent(DryerWorkflowEvent @event)
    {
        Events.Add(@event);
    }
}

public class DryerWorkflowEvent
{
    public string Name { get; set; }
    public DryerWorkflow.State PreviousState { get; set; }
    public DryerWorkflow.State NextState { get; set; }
    public DryerWorkflow.Trigger Trigger { get; set; }
    public EventType Type { get; set; }

    public override string ToString()
    {
        return $"{Name} {PreviousState} -> {Trigger} -> {NextState}";
    }
}

public enum EventType
{
    OnEntry,
    OnLeave, 
    OnTransitionCompleted,
    OnTransitioned,
    OnExit,
    OnUnhandledTrigger,
    OnEntryFrom,
    TransitionGuard
}