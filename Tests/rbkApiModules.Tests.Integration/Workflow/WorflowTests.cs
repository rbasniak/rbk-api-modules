using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Tests.Integration.Workflow;

public class WorflowTests
{
    [Fact]
    public void Test()
    {
        var wf1 = new ChangeRequestWorkflow(new FakeNotificationService());

        Debug.WriteLine($"ChangeRequest is in State {wf1._machine.State}");

        // wf1._machine.Fire(ChangeRequestWorkflow.Trigger.ENVIAR_PARA_AVALIACAO);
        wf1.EnterThatState(new ChangeRequest { Id = "019" }, DateTime.Now);

        wf1._machine.Activate(); // TODO: check if it's possible to remove the Activate and their events

        Debug.WriteLine($"ChangeRequest is in State {wf1._machine.State}");
        
        wf1._machine.Fire(ChangeRequestWorkflow.Trigger.APROVAR_PELO_ADMINISTRADOR);

        Debug.WriteLine($"ChangeRequest is in State {wf1._machine.State}");

        var temp1 = wf1._machine.GetDetailedPermittedTriggers();
        var temp2 = wf1._machine.GetInfo();
        var temp3 = wf1._machine.PermittedTriggers;
        var temp4 = wf1._machine.State;

        var graph = UmlDotGraph<ChangeRequestWorkflow.State, ChangeRequestWorkflow.Trigger>.Format(wf1._machine.GetInfo());

        //var wf2 = new ChangeRequestWorkflow();

        //wf2._machine.Fire(ChangeRequestWorkflow.Trigger.ENVIAR_PARA_AVALIACAO);
        //wf2._machine.Fire(ChangeRequestWorkflow.Trigger.ENVIAR_PARA_AVALIACAO);
    }
}

public class FakeNotificationService
{
    public void SendRequestReceived(ChangeRequest request)
    {
        Debug.WriteLine($"Notification sent to {request.CurrentOwner} telling that the request {request.InternalNumber} was received.");
    }
}