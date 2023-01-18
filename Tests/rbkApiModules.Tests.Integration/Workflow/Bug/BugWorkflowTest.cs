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
        var bug = new Bug
        {
            Title = "New Bug"
        };

        var workflow = new BugWorkflow(bug);

        workflow.Assign("John");
        workflow.Defer();
        workflow.Assign("Jane");
        workflow.Assign("John");
        workflow.Close();
    }

    [FriendlyNamedFact("IT-001")]
    public void Document_Workflow_Should_Generate_Dot_Graph()
    {
        var workflow = new BugWorkflow(new Bug());

        var graph = UmlDotGraph<State, Trigger>.Format(workflow._machine.GetInfo());

        graph.ShouldBe("""
            digraph {
            compound=true;
            node [shape=Mrecord]
            rankdir="LR"

            subgraph "clusterOpen"
            	{
            	label = "Open"
            "Assigned" [label="Assigned|exit / Function"];
            }
            "Deferred" [label="Deferred|entry / Function"];
            "Closed" [label="Closed"];

            "Open" -> "Assigned" [style="solid", label="Assign / OnAssigned"];
            "Assigned" -> "Assigned" [style="solid", label="Assign"];
            "Assigned" -> "Closed" [style="solid", label="Close"];
            "Assigned" -> "Deferred" [style="solid", label="Defer"];
            "Deferred" -> "Assigned" [style="solid", label="Assign / OnAssigned"];
             init [label="", shape=point];
             init -> "Open"[style = "solid"]
            }
            """);
    }
}

