using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using State = rbkApiModules.Tests.Integration.Workflow.Document.Async.AsyncDocumentWorkflow.State;
using Trigger = rbkApiModules.Tests.Integration.Workflow.Document.Async.AsyncDocumentWorkflow.Trigger;

namespace rbkApiModules.Tests.Integration.Workflow.Document.Async;


public class AsyncDocumentWorkflowTests
{
    [FriendlyNamedFact("IT-000")]
    public void Document_Workflow_Should_Finish_With_Async_Events()
    {
        var document = new Document
        {
            Creator = "CREATOR",
            Number = 19,
            CurrentOwner = "CREATOR"
        };

        document.State.ShouldBe(State.DRAFT);

        var workflow = new AsyncDocumentWorkflow(document);

        workflow.Dispatch(Trigger.UPDATE);
        document.State.ShouldBe(State.DRAFT);
        Thread.Sleep(1000);
        document.Events.PreviousLastEventsShouldBe(State.DRAFT, Trigger.UPDATE, State.DRAFT);

        workflow.Dispatch(Trigger.DECLINE);

        document.State.ShouldBe(State.DRAFT);
        Thread.Sleep(1000);
        document.Events.Last().Trigger.ShouldBe(Trigger.DECLINE);
        document.Events.Last().Type.ShouldBe(EventType.OnUnhandledTrigger);
        document.Events.Last().PreviousState.ShouldBe(State.DRAFT);
        document.Events.Last().NextState.ShouldBe(State.DRAFT);

        workflow.Dispatch(Trigger.BEGIN_REVIEW);
        Thread.Sleep(1000);
        document.State.ShouldBe(State.REVIEW);
        document.Events.PreviousLastEventsShouldBe(State.DRAFT, Trigger.BEGIN_REVIEW, State.REVIEW);

        workflow.Dispatch(Trigger.CHANGE_NEEDED);
        Thread.Sleep(1000);
        document.State.ShouldBe(State.CHANGE_REQUESTED);
        document.Events.PreviousLastEventsShouldBe(State.REVIEW, Trigger.CHANGE_NEEDED, State.CHANGE_REQUESTED);

        workflow.Dispatch(Trigger.REJECT);
        Thread.Sleep(1000);
        document.State.ShouldBe(State.REVIEW);
        document.Events.PreviousLastEventsShouldBe(State.CHANGE_REQUESTED, Trigger.REJECT, State.REVIEW);

        workflow.Dispatch(Trigger.CHANGE_NEEDED);
        Thread.Sleep(1000);
        document.State.ShouldBe(State.CHANGE_REQUESTED);
        document.Events.PreviousLastEventsShouldBe(State.REVIEW, Trigger.CHANGE_NEEDED, State.CHANGE_REQUESTED);

        workflow.Dispatch(Trigger.ACCEPT);
        Thread.Sleep(1000);
        document.State.ShouldBe(State.DRAFT);
        document.Events.PreviousLastEventsShouldBe(State.CHANGE_REQUESTED, Trigger.ACCEPT, State.DRAFT);

        workflow.Dispatch(Trigger.UPDATE);
        Thread.Sleep(1000);
        document.State.ShouldBe(State.DRAFT);
        document.Events.PreviousLastEventsShouldBe(State.DRAFT, Trigger.UPDATE, State.DRAFT);

        workflow.Dispatch(Trigger.BEGIN_REVIEW);
        Thread.Sleep(1000);
        document.State.ShouldBe(State.REVIEW);
        document.Events.PreviousLastEventsShouldBe(State.DRAFT, Trigger.BEGIN_REVIEW, State.REVIEW);

        workflow.Dispatch(Trigger.SUBMIT);
        Thread.Sleep(1000);
        document.State.ShouldBe(State.SUBMITTED_TO_CLIENT);
        document.Events.PreviousLastEventsShouldBe(State.REVIEW, Trigger.SUBMIT, State.SUBMITTED_TO_CLIENT);

        workflow.Dispatch(Trigger.DECLINE);
        Thread.Sleep(1000);
        document.State.ShouldBe(State.DECLINED);
        document.Events.PreviousLastEventsShouldBe(State.SUBMITTED_TO_CLIENT, Trigger.DECLINE, State.DECLINED);

        workflow.Dispatch(Trigger.RESTART_REVIEW);
        Thread.Sleep(1000);
        document.State.ShouldBe(State.REVIEW);
        document.Events.PreviousLastEventsShouldBe(State.DECLINED, Trigger.RESTART_REVIEW, State.REVIEW);

        workflow.Dispatch(Trigger.SUBMIT);
        Thread.Sleep(1000);
        document.State.ShouldBe(State.SUBMITTED_TO_CLIENT);
        document.Events.PreviousLastEventsShouldBe(State.REVIEW, Trigger.SUBMIT, State.SUBMITTED_TO_CLIENT);

        workflow.Dispatch(Trigger.APPROVE);
        Thread.Sleep(1000);
        document.State.ShouldBe(State.APPROVED);
        document.Events.PreviousLastEventsShouldBe(State.SUBMITTED_TO_CLIENT, Trigger.APPROVE, State.APPROVED);
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