using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Tests.Integration.Workflow.Bug;

public class BugWorkflow
{
    public enum State { Open, Assigned, Doing, Deferred, Closed }

    public enum Trigger { Assign, Start, Stop, Defer, Close }

    public readonly StateMachine<State, Trigger> _machine;

    private readonly Bug _bug;

    public BugWorkflow(Bug bug)
    {
        _bug = bug;

        _machine = new StateMachine<State, Trigger>(_bug.State);

        _machine.OnTransitionCompleted(t => 
        {
            _bug.State = t.Destination;

            _bug.AddEvent(new BugWorkflowEvent
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
            _bug.AddEvent(new BugWorkflowEvent
            {
                Name = "GLOBAL_EVENT::OnTransitioned",
                PreviousState = t.Source,
                NextState = t.Destination,
                Trigger = t.Trigger,
                Type = EventType.OnTransitioned
            });
        });

        _machine.Configure(State.Open)
            .Permit(Trigger.Assign, State.Assigned)
            .OnEntry(x => _bug.AddEvent(new BugWorkflowEvent
            {
                Name = "OPEN:OnEntry",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnEntry
            }), "OPEN:OnEntry")
            .OnExit(x => _bug.AddEvent(new BugWorkflowEvent
            {
                Name = "OPEN:OnExit",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnExit
            }), "OPEN:OnExit");

        _machine.Configure(State.Assigned)
            .PermitReentry(Trigger.Assign)
            .Permit(Trigger.Defer, State.Deferred)
            .Permit(Trigger.Start, State.Doing)
            .Permit(Trigger.Close, State.Closed)
            .OnEntryFrom(Trigger.Assign, x => _bug.AddEvent(new BugWorkflowEvent
            {
                Name = "ASSIGNED:OnEntryFrom",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnEntryFrom
            }), "ASSIGNED:OnEntryFrom")  
            .OnEntry(x => _bug.AddEvent(new BugWorkflowEvent
            {
                Name = "ASSIGNED:OnEntry",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnEntry
            }), "ASSIGNED:OnEntry")
            .OnExit(x => _bug.AddEvent(new BugWorkflowEvent
            {
                Name = "ASSIGNED:OnExit",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnExit
            }), "ASSIGNED:OnExit");

        _machine.Configure(State.Doing)
            .Permit(Trigger.Stop, State.Assigned)
            .OnEntry(x => _bug.AddEvent(new BugWorkflowEvent
            {
                Name = "DOING:OnEntry",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnEntry
            }), "DOING:OnEntry")
            .OnExit(x => _bug.AddEvent(new BugWorkflowEvent
            {
                Name = "DOING:OnExit",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnExit
            }), "DOING:OnExit");

        _machine.Configure(State.Deferred)
            .OnEntry(() => _bug.Assignee = null)
            .Permit(Trigger.Assign, State.Assigned)
            .OnEntry(x => _bug.AddEvent(new BugWorkflowEvent
            {
                Name = "DEFERRED:OnEntry",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnEntry
            }), "DEFERRED:OnEntry")
            .OnExit(x => _bug.AddEvent(new BugWorkflowEvent
            {
                Name = "DEFERRED:OnExit",
                PreviousState = x.Source,
                NextState = x.Destination,
                Trigger = x.Trigger,
                Type = EventType.OnExit
            }), "DEFERRED:OnExit");
    }  

    public void Close()
    {
        _machine.Fire(Trigger.Close);
    }

    public void Assign(string assignee)
    {
        _machine.Fire(Trigger.Assign, new object[] { assignee });
    }

    public bool CanAssign => _machine.CanFire(Trigger.Assign);

    public void Defer()
    {
        _machine.Fire(Trigger.Defer);
    }

    public void Start()
    {
        _machine.Fire(Trigger.Start);
    }

    public void Stop()
    {
        _machine.Fire(Trigger.Stop);
    } 

    private void SendEmailToAssignee(string message)
    {
        Console.WriteLine("{0}, RE {1}: {2}", _bug.Assignee, _bug.Title, message);
    } 
}

public class Bug
{
    public Bug(string title)
    {
        Title = title; 
        State = BugWorkflow.State.Open;
        Events = new List<BugWorkflowEvent>();
    }

    public BugWorkflow.State State { get; set; }
    public string Title { get; set; }
    public string Assignee { get; set; }
    public List<BugWorkflowEvent> Events { get; set; }

    internal void AddEvent(BugWorkflowEvent @event)
    {
        Events.Add(@event);
    }
}

public class BugWorkflowEvent
{
    public string Name { get; set; }
    public BugWorkflow.State PreviousState { get; set; }
    public BugWorkflow.State NextState { get; set; }
    public BugWorkflow.Trigger Trigger { get; set; }
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
    OnEntryFrom
}