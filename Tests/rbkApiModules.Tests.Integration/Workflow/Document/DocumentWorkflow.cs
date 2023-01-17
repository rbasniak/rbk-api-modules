using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Tests.Integration.Workflow.Document;

public class DocumentWorkflow
{
    public readonly StateMachine<State, Trigger> _machine;

    public enum Trigger
    {
        BEGIN_REVIEW,
        UPDATE,
        CHANGE_NEEDED,
        ACCEPT,
        REJECT,
        SUBMIT,
        DECLINE,
        APPROVE,
        RESTART_REVIEW
    }

    public enum State
    {
        DRAFT,
        REVIEW,
        CHANGE_REQUESTED,
        DECLINED,
        SUBMITTED_TO_CLIENT,
        APPROVED
    }

    private Document _document;

    // https://www.lloydatkinson.net/posts/2022/modelling-workflows-with-finite-state-machines-in-dotnet/
    public DocumentWorkflow(Document document)
    {
        _document = document;

        _machine = new StateMachine<State, Trigger>(State.DRAFT);

        _machine.OnTransitionCompleted(
            (transition) => EventStore.Enqueue(
                new DocumentWorkflowEvent 
                { 
                    PreviousState = transition.Source, 
                    NextState = transition.Destination, 
                    Trigger = transition.Trigger, 
                    Type = EventType.OnTransitionCompleted 
                }));

        _machine.OnTransitioned(
            (transition) => EventStore.Enqueue(
                new DocumentWorkflowEvent
                {
                    PreviousState = transition.Source,
                    NextState = transition.Destination,
                    Trigger = transition.Trigger,
                    Type = EventType.OnTransitioned
                }));

        _machine.Configure(State.DRAFT)
            .PermitReentry(Trigger.UPDATE)
            .Permit(Trigger.BEGIN_REVIEW, State.REVIEW)
            .OnEntry(
                () => EventStore.Enqueue(
                    new DocumentWorkflowEvent 
                    { 
                        PreviousState = State.DRAFT, 
                        NextState = State.DRAFT, 
                        Trigger = Trigger.UPDATE, 
                        Type = EventType.OnEntry 
                    }))
            .OnExit(
                () => EventStore.Enqueue(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = State.DRAFT,
                        NextState = State.DRAFT,
                        Trigger = Trigger.UPDATE,
                        Type = EventType.OnExit
                    }));

        _machine.Configure(State.REVIEW)
            .Permit(Trigger.CHANGE_NEEDED, State.CHANGE_REQUESTED)
            .Permit(Trigger.SUBMIT, State.SUBMITTED_TO_CLIENT);

        _machine.Configure(State.CHANGE_REQUESTED)
            .Permit(Trigger.ACCEPT, State.DRAFT)
            .Permit(Trigger.REJECT, State.REVIEW);

        _machine.Configure(State.SUBMITTED_TO_CLIENT)
            .Permit(Trigger.APPROVE, State.APPROVED)
            .Permit(Trigger.DECLINE, State.DECLINED);

        _machine.Configure(State.DECLINED)
            .Permit(Trigger.RESTART_REVIEW, State.REVIEW);

        _machine.Configure(State.APPROVED);
    }

    internal void Dispatch(Trigger trigger)
    {
        _machine.Fire(trigger);
    }
}

public static class EventStore
{
    static EventStore()
    {
        Events = new List<DocumentWorkflowEvent>();
    }

    public static List<DocumentWorkflowEvent> Events { get; set; }

    public static void Enqueue(DocumentWorkflowEvent @event)
    {
        Events.Add(@event); 
    }
}

public class DocumentWorkflowEvent
{
    public DocumentWorkflow.State PreviousState { get; set; }
    public DocumentWorkflow.State NextState { get; set; }
    public DocumentWorkflow.Trigger Trigger { get; set; }
    public EventType Type { get; set; }
}

public enum EventType
{
    OnEntry,
    OnLeave,
    OnActivate,
    OnDeactivate,
    OnEntryAsync,
    OnLeaveAsync,
    OnActivateAsync,
    OnDeactivateAsync,
    OnTransitionCompleted,
    OnTransitioned,
    OnExit
}

public class Document
{
    public string CurrentOwner { get; set; }
    public string Creator { get; set; }
    public string Reviewer { get; set; }
    public string Approver { get; set; }
    public int Number { get; set; }
    public DocumentWorkflow.State State { get; set; }
}

