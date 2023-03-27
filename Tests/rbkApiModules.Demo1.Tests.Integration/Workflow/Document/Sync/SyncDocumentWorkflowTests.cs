using Stateless.Graph;
using State = rbkApiModules.Demo1.Tests.Integration.Workflow.Document.Sync.SyncDocumentWorkflow.State;
using Trigger = rbkApiModules.Demo1.Tests.Integration.Workflow.Document.Sync.SyncDocumentWorkflow.Trigger;

namespace rbkApiModules.Demo1.Tests.Integration.Workflow.Document.Sync;

public class SyncDocumentWorkflowTests
{
    [FriendlyNamedFact("IT-000")]
    public void Document_Workflow_Should_Finish_With_Sync_Events()
    {
        var document = new Document
        {
            Creator = "CREATOR",
            Number = 19,
            CurrentOwner = "CREATOR"
        };

        document.State.ShouldBe(State.DRAFT);

        var workflow = new SyncDocumentWorkflow(document);

        workflow.Dispatch(Trigger.UPDATE);
        document.State.ShouldBe(State.DRAFT);
        document.Events.PreviousLastEventsShouldBe(State.DRAFT, Trigger.UPDATE, State.DRAFT);

        workflow.Dispatch(Trigger.DECLINE);

        document.State.ShouldBe(State.DRAFT);
        document.Events.Last().Trigger.ShouldBe(Trigger.DECLINE);
        document.Events.Last().Type.ShouldBe(EventType.OnUnhandledTrigger);
        document.Events.Last().PreviousState.ShouldBe(State.DRAFT);
        document.Events.Last().NextState.ShouldBe(State.DRAFT);

        workflow.Dispatch(Trigger.BEGIN_REVIEW);
        document.State.ShouldBe(State.REVIEW);
        document.Events.PreviousLastEventsShouldBe(State.DRAFT, Trigger.BEGIN_REVIEW, State.REVIEW);

        workflow.Dispatch(Trigger.CHANGE_NEEDED);
        document.State.ShouldBe(State.CHANGE_REQUESTED);
        document.Events.PreviousLastEventsShouldBe(State.REVIEW, Trigger.CHANGE_NEEDED, State.CHANGE_REQUESTED);

        workflow.Dispatch(Trigger.REJECT);
        document.State.ShouldBe(State.REVIEW);
        document.Events.PreviousLastEventsShouldBe(State.CHANGE_REQUESTED, Trigger.REJECT, State.REVIEW);

        workflow.Dispatch(Trigger.CHANGE_NEEDED);
        document.State.ShouldBe(State.CHANGE_REQUESTED);
        document.Events.PreviousLastEventsShouldBe(State.REVIEW, Trigger.CHANGE_NEEDED, State.CHANGE_REQUESTED);

        workflow.Dispatch(Trigger.ACCEPT);
        document.State.ShouldBe(State.DRAFT);
        document.Events.PreviousLastEventsShouldBe(State.CHANGE_REQUESTED, Trigger.ACCEPT, State.DRAFT);

        workflow.Dispatch(Trigger.UPDATE);
        document.State.ShouldBe(State.DRAFT);
        document.Events.PreviousLastEventsShouldBe(State.DRAFT, Trigger.UPDATE, State.DRAFT);

        workflow.Dispatch(Trigger.BEGIN_REVIEW);
        document.State.ShouldBe(State.REVIEW);
        document.Events.PreviousLastEventsShouldBe(State.DRAFT, Trigger.BEGIN_REVIEW, State.REVIEW);

        workflow.Dispatch(Trigger.SUBMIT);
        document.State.ShouldBe(State.SUBMITTED_TO_CLIENT);
        document.Events.PreviousLastEventsShouldBe(State.REVIEW, Trigger.SUBMIT, State.SUBMITTED_TO_CLIENT);

        workflow.Dispatch(Trigger.DECLINE);
        document.State.ShouldBe(State.DECLINED);
        document.Events.PreviousLastEventsShouldBe(State.SUBMITTED_TO_CLIENT, Trigger.DECLINE, State.DECLINED);

        workflow.Dispatch(Trigger.RESTART_REVIEW);
        document.State.ShouldBe(State.REVIEW);
        document.Events.PreviousLastEventsShouldBe(State.DECLINED, Trigger.RESTART_REVIEW, State.REVIEW);

        workflow.Dispatch(Trigger.SUBMIT);
        document.State.ShouldBe(State.SUBMITTED_TO_CLIENT);
        document.Events.PreviousLastEventsShouldBe(State.REVIEW, Trigger.SUBMIT, State.SUBMITTED_TO_CLIENT);

        workflow.Dispatch(Trigger.APPROVE);
        document.State.ShouldBe(State.APPROVED);
        document.Events.PreviousLastEventsShouldBe(State.SUBMITTED_TO_CLIENT, Trigger.APPROVE, State.APPROVED);
    }

    [FriendlyNamedFact("IT-001")]
    public void Document_Workflow_Should_Generate_Dot_Graph()
    {
        var workflow = new SyncDocumentWorkflow(new Document());

        var graph = UmlDotGraph<State, Trigger>.Format(workflow._machine.GetInfo());

        graph.ShouldBe("""
            digraph {
            compound=true;
            node [shape=Mrecord]
            rankdir="LR"
            "DRAFT" [label="DRAFT|entry / DRAFT::OnEnter\nexit / DRAFT::OnExit"];
            "REVIEW" [label="REVIEW|entry / REVIEW::OnEnter\nexit / REVIEW::OnExit"];
            "CHANGE_REQUESTED" [label="CHANGE_REQUESTED|entry / CHANGE_REQUESTED::OnEnter\nexit / CHANGE_REQUESTED::OnExit"];
            "SUBMITTED_TO_CLIENT" [label="SUBMITTED_TO_CLIENT|entry / SUBMITTED_TO_CLIENT::OnEnter\nexit / SUBMITTED_TO_CLIENT::OnExit"];
            "DECLINED" [label="DECLINED|entry / DECLINED::OnEnter\nexit / DECLINED::OnExit"];
            "APPROVED" [label="APPROVED|entry / APPROVED::OnEnter\nexit / APPROVED::OnExit"];

            "DRAFT" -> "DRAFT" [style="solid", label="UPDATE / DRAFT::OnEnter"];
            "DRAFT" -> "REVIEW" [style="solid", label="BEGIN_REVIEW"];
            "REVIEW" -> "CHANGE_REQUESTED" [style="solid", label="CHANGE_NEEDED"];
            "REVIEW" -> "SUBMITTED_TO_CLIENT" [style="solid", label="SUBMIT"];
            "CHANGE_REQUESTED" -> "DRAFT" [style="solid", label="ACCEPT"];
            "CHANGE_REQUESTED" -> "REVIEW" [style="solid", label="REJECT"];
            "SUBMITTED_TO_CLIENT" -> "APPROVED" [style="solid", label="APPROVE"];
            "SUBMITTED_TO_CLIENT" -> "DECLINED" [style="solid", label="DECLINE"];
            "DECLINED" -> "REVIEW" [style="solid", label="RESTART_REVIEW"];
             init [label="", shape=point];
             init -> "DRAFT"[style = "solid"]
            }
            """);
    }
}

public static class EventStoreExtensions
{
    public static void PreviousLastEventsShouldBe(this List<DocumentWorkflowEvent> store, State fromState, Trigger trigger, State toState)
    {
        var eventCount = store.Count;

        store[eventCount - 4].Trigger.ShouldBe(trigger);
        store[eventCount - 4].Type.ShouldBe(EventType.OnExit);
        store[eventCount - 4].PreviousState.ShouldBe(fromState);
        store[eventCount - 4].NextState.ShouldBe(toState);

        store[eventCount - 3].Trigger.ShouldBe(trigger);
        store[eventCount - 3].Type.ShouldBe(EventType.OnTransitioned);
        store[eventCount - 3].PreviousState.ShouldBe(fromState);
        store[eventCount - 3].NextState.ShouldBe(toState);

        store[eventCount - 2].Trigger.ShouldBe(trigger);
        store[eventCount - 2].Type.ShouldBe(EventType.OnEntry);
        store[eventCount - 2].PreviousState.ShouldBe(fromState);
        store[eventCount - 2].NextState.ShouldBe(toState);

        store[eventCount - 1].Trigger.ShouldBe(trigger);
        store[eventCount - 1].Type.ShouldBe(EventType.OnTransitionCompleted);
        store[eventCount - 1].PreviousState.ShouldBe(fromState);
        store[eventCount - 1].NextState.ShouldBe(toState);
    }
}