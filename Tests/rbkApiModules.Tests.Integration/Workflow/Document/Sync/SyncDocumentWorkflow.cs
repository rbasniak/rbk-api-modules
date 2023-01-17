using Stateless;

namespace rbkApiModules.Tests.Integration.Workflow.Document.Sync;

public class SyncDocumentWorkflow
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
    public SyncDocumentWorkflow(Document document)
    {
        _document = document;

        _machine = new StateMachine<State, Trigger>(State.DRAFT);

        _machine.OnTransitionCompleted(
            (transition) =>
            {
                _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = transition.Source,
                        NextState = transition.Destination,
                        Trigger = transition.Trigger,
                        Type = EventType.OnTransitionCompleted
                    });

                document.State = transition.Destination;
            });

        _machine.OnTransitioned(
            (transition) => _document.Events.Add(
                new DocumentWorkflowEvent
                {
                    PreviousState = transition.Source,
                    NextState = transition.Destination,
                    Trigger = transition.Trigger,
                    Type = EventType.OnTransitioned
                }));

        _machine.OnUnhandledTrigger(
            (state, trigger) => _document.Events.Add(
                new DocumentWorkflowEvent
                {
                    PreviousState = state,
                    NextState = state,
                    Trigger = trigger,
                    Type = EventType.OnUnhandledTrigger
                }));

        _machine.Configure(State.DRAFT)
            .PermitReentry(Trigger.UPDATE)
            .Permit(Trigger.BEGIN_REVIEW, State.REVIEW)
            .OnEntry(
                t => _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = t.Source,
                        NextState = t.Destination,
                        Trigger = t.Trigger,
                        Type = EventType.OnEntry
                    }), "DRAFT::OnEnter")
            .OnExit(
                t => _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = t.Source,
                        NextState = t.Destination,
                        Trigger = t.Trigger,
                        Type = EventType.OnExit
                    }), "DRAFT::OnExit");

        _machine.Configure(State.REVIEW)
            .Permit(Trigger.CHANGE_NEEDED, State.CHANGE_REQUESTED)
            .Permit(Trigger.SUBMIT, State.SUBMITTED_TO_CLIENT)
            .OnEntry(
                t => _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = t.Source,
                        NextState = t.Destination,
                        Trigger = t.Trigger,
                        Type = EventType.OnEntry
                    }), "REVIEW::OnEnter")
            .OnExit(
                t => _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = t.Source,
                        NextState = t.Destination,
                        Trigger = t.Trigger,
                        Type = EventType.OnExit
                    }), "REVIEW::OnExit");

        _machine.Configure(State.CHANGE_REQUESTED)
            .Permit(Trigger.ACCEPT, State.DRAFT)
            .Permit(Trigger.REJECT, State.REVIEW)
            .OnEntry(
                t => _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = t.Source,
                        NextState = t.Destination,
                        Trigger = t.Trigger,
                        Type = EventType.OnEntry
                    }), "CHANGE_REQUESTED::OnEnter")
            .OnExit(
                t => _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = t.Source,
                        NextState = t.Destination,
                        Trigger = t.Trigger,
                        Type = EventType.OnExit
                    }), "CHANGE_REQUESTED::OnExit");

        _machine.Configure(State.SUBMITTED_TO_CLIENT)
            .Permit(Trigger.APPROVE, State.APPROVED)
            .Permit(Trigger.DECLINE, State.DECLINED)
            .OnEntry(
                t => _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = t.Source,
                        NextState = t.Destination,
                        Trigger = t.Trigger,
                        Type = EventType.OnEntry
                    }), "SUBMITTED_TO_CLIENT::OnEnter")
            .OnExit(
                t => _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = t.Source,
                        NextState = t.Destination,
                        Trigger = t.Trigger,
                        Type = EventType.OnExit
                    }), "SUBMITTED_TO_CLIENT::OnExit");

        _machine.Configure(State.DECLINED)
            .Permit(Trigger.RESTART_REVIEW, State.REVIEW)
            .OnEntry(
                t => _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = t.Source,
                        NextState = t.Destination,
                        Trigger = t.Trigger,
                        Type = EventType.OnEntry
                    }), "DECLINED::OnEnter")
            .OnExit(
                t => _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = t.Source,
                        NextState = t.Destination,
                        Trigger = t.Trigger,
                        Type = EventType.OnExit
                    }), "DECLINED::OnExit");

        _machine.Configure(State.APPROVED)
            .OnEntry(
                t => _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = t.Source,
                        NextState = t.Destination,
                        Trigger = t.Trigger,
                        Type = EventType.OnEntry
                    }), "APPROVED::OnEnter")
            .OnExit(
                t => _document.Events.Add(
                    new DocumentWorkflowEvent
                    {
                        PreviousState = t.Source,
                        NextState = t.Destination,
                        Trigger = t.Trigger,
                        Type = EventType.OnExit
                    }), "APPROVED::OnExit");
    }

    internal void Dispatch(Trigger trigger)
    {
        _machine.Fire(trigger);
    }
}

public class DocumentWorkflowEvent
{
    public SyncDocumentWorkflow.State PreviousState { get; set; }
    public SyncDocumentWorkflow.State NextState { get; set; }
    public SyncDocumentWorkflow.Trigger Trigger { get; set; }
    public EventType Type { get; set; }

    public override string ToString()
    {
        return $"{Type} {PreviousState} -> {Trigger} -> {NextState}";
    }
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
    OnExit,
    OnUnhandledTrigger
}

public class Document
{
    public Document()
    {
        Events = new List<DocumentWorkflowEvent>();
        State = SyncDocumentWorkflow.State.DRAFT;
    }

    public string CurrentOwner { get; set; }
    public string Creator { get; set; }
    public string Reviewer { get; set; }
    public string Approver { get; set; }
    public int Number { get; set; }
    public SyncDocumentWorkflow.State State { get; set; }
    public List<DocumentWorkflowEvent> Events { get; set; }
}

