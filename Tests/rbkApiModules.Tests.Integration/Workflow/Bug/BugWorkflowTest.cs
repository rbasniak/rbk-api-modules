using rbkApiModules.Tests.Integration.Workflow.Document.Sync;
using Stateless;
using Stateless.Graph;
using State = rbkApiModules.Tests.Integration.Workflow.Bug.BugWorkflow.State;
using Trigger = rbkApiModules.Tests.Integration.Workflow.Bug.BugWorkflow.Trigger;

namespace rbkApiModules.Tests.Integration.Workflow.Bug;

public class BugWorkflowTests
{
    [FriendlyNamedFact("IT-000")]
    public void Bug_Workflow_Should_Finish_With_Sync_Events()
    {
        var bug = new Bug("New Bug");

        var workflow = new BugWorkflow(bug);

        workflow.Assign("John");
        workflow.Defer();
        workflow.Assign("Jane");
        workflow.Start();
        workflow.Stop();
        workflow.Assign("John");
        workflow.Close();
    }

    [FriendlyNamedFact("IT-000")]
    public void Bug_Workflow_Should_Continue_When_Entity_Is_Already_In_The_Workflow()
    {
        var bug = new Bug("New Bug") { State = State.Assigned, Assignee = "John" };

        var workflow = new BugWorkflow(bug);

        workflow.Close();
    }

    [FriendlyNamedFact("IT-000")]
    public void Document_Workflow_Should_Generate_Dot_Graph()
    {
        var workflow = new BugWorkflow(new Bug(String.Empty));

        var graph = UmlDotGraph<State, Trigger>.Format(workflow._machine.GetInfo());

        graph.ShouldBe("""
            digraph {
            compound=true;
            node [shape=Mrecord]
            rankdir="LR"

            subgraph "clusterOpen"
            	{
            	label = "Open\n----------\nentry / OPEN:OnEntry\nexit / OPEN:OnExit"
            "Assigned" [label="Assigned|entry / ASSIGNED:OnEntry\nexit / ASSIGNED:OnExit"];
            "Doing" [label="Doing|entry / DOING:OnEntry\nexit / DOING:OnExit"];
            }
            "Deferred" [label="Deferred|entry / Function\nentry / DEFERRED:OnEntry\nexit / DEFERRED:OnExit"];
            "Closed" [label="Closed"];

            "Open" -> "Assigned" [style="solid", label="Assign / ASSIGNED:OnEntryFrom"];
            "Assigned" -> "Assigned" [style="solid", label="Assign / ASSIGNED:OnEntry"];
            "Assigned" -> "Deferred" [style="solid", label="Defer"];
            "Assigned" -> "Doing" [style="solid", label="Start"];
            "Assigned" -> "Closed" [style="solid", label="Close"];
            "Doing" -> "Assigned" [style="solid", label="Stop"];
            "Deferred" -> "Assigned" [style="solid", label="Assign / ASSIGNED:OnEntryFrom"];
             init [label="", shape=point];
             init -> "Open"[style = "solid"]
            }
            """);
    }
}

