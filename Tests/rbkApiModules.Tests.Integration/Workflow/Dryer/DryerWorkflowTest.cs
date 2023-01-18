using rbkApiModules.Tests.Integration.Workflow.Document.Sync;
using Stateless;
using Stateless.Graph;
using State = rbkApiModules.Tests.Integration.Workflow.Dryer.DryerWorkflow.State;
using Trigger = rbkApiModules.Tests.Integration.Workflow.Dryer.DryerWorkflow.Trigger;

namespace rbkApiModules.Tests.Integration.Workflow.Dryer;

public class DryerWorkflowTests
{
    [FriendlyNamedFact("IT-000")]
    public void Dryer_Workflow_Should_Finish_With_Sync_Events()
    {
        var dryer = new Dryer();

        var workflow = new DryerWorkflow(dryer);

        workflow.TurnOn(1);
        workflow.TurnOff();

        var exception = Should.Throw<InvalidOperationException>(() => workflow.TurnOn(5));

        exception.Message.ShouldContain("Electricity price guard");
    } 

    [FriendlyNamedFact("IT-000")]
    public void Dryer_Workflow_Should_Generate_Dot_Graph()
    {
        var workflow = new DryerWorkflow(new Dryer());

        var graph = UmlDotGraph<State, Trigger>.Format(workflow._machine.GetInfo());

        graph.ShouldBe("""
            digraph {
            compound=true;
            node [shape=Mrecord]
            rankdir="LR"
            "Off" [label="Off|entry / OFF:OnEntry\nexit / OFF:OnExit"];
            "On" [label="On|entry / ON:OnEntry\nexit / ON:OnExit"];

            "Off" -> "On" [style="solid", label="TurnOn [Electricity price guard]"];
            "On" -> "Off" [style="solid", label="TurnOff"];
             init [label="", shape=point];
             init -> "Off"[style = "solid"]
            }
            """);
    }
}

