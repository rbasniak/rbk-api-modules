using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Tests.Integration.Workflow.Document;

public class DocumentWorkflowTests
{
    [Fact]
    public void Test()
    {
        var document = new Document 
        { 
            State = DocumentWorkflow.State.DRAFT,
            Creator = "CREATOR",
            Number = 19,
            CurrentOwner = "CREATOR"
        };

        var wf1 = new DocumentWorkflow(document);

        wf1.Dispatch(DocumentWorkflow.Trigger.UPDATE);


        EventStore.Events[0].Trigger.ShouldBe(DocumentWorkflow.Trigger.UPDATE);
        EventStore.Events[0].Type.ShouldBe(EventType.OnExit);
        EventStore.Events[0].PreviousState.ShouldBe(DocumentWorkflow.State.DRAFT);
        EventStore.Events[0].NextState.ShouldBe(DocumentWorkflow.State.DRAFT);

        EventStore.Events[1].Trigger.ShouldBe(DocumentWorkflow.Trigger.UPDATE);
        EventStore.Events[1].Type.ShouldBe(EventType.OnTransitioned);
        EventStore.Events[1].PreviousState.ShouldBe(DocumentWorkflow.State.DRAFT);
        EventStore.Events[1].NextState.ShouldBe(DocumentWorkflow.State.DRAFT);

        EventStore.Events[2].Trigger.ShouldBe(DocumentWorkflow.Trigger.UPDATE);
        EventStore.Events[2].Type.ShouldBe(EventType.OnEntry);
        EventStore.Events[2].PreviousState.ShouldBe(DocumentWorkflow.State.DRAFT);
        EventStore.Events[2].NextState.ShouldBe(DocumentWorkflow.State.DRAFT);

        EventStore.Events[3].Trigger.ShouldBe(DocumentWorkflow.Trigger.UPDATE);
        EventStore.Events[3].Type.ShouldBe(EventType.OnTransitionCompleted);
        EventStore.Events[3].PreviousState.ShouldBe(DocumentWorkflow.State.DRAFT);
        EventStore.Events[3].NextState.ShouldBe(DocumentWorkflow.State.DRAFT);
    }
}