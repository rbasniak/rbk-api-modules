using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Tests.Integration.Workflow.Bug;

public class BugWorkflow
{
    public enum State { Open, Assigned, Deferred, Closed }

    public enum Trigger { Assign, Defer, Close }

    public readonly StateMachine<State, Trigger> _machine;

    private readonly Bug _bug;

    public BugWorkflow(Bug bug)
    {
        _bug = bug;

        _machine = new StateMachine<State, Trigger>(_bug.State);

        _machine.OnTransitionCompleted(t => _bug.State = t.Destination);

        _machine.Configure(State.Open)
            .Permit(Trigger.Assign, State.Assigned);

        _machine.Configure(State.Assigned)
            .SubstateOf(State.Open)
            .OnEntryFrom(Trigger.Assign, OnAssigned)  
            .PermitReentry(Trigger.Assign)
            .Permit(Trigger.Close, State.Closed)
            .Permit(Trigger.Defer, State.Deferred)
            .OnExit(t => SendEmailToAssignee("You're off the hook."));

        _machine.Configure(State.Deferred)
            .OnEntry(() => _bug.Assignee = null)
            .Permit(Trigger.Assign, State.Assigned);
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
    /// <summary>
    /// This method is called automatically when the Assigned state is entered, but only when the trigger is _assignTrigger.
    /// </summary>
    /// <param name="assignee"></param>
    private void OnAssigned(Transition<State, Trigger> transition)
    {
        var assignee = transition.Parameters[0].ToString();

        if (_bug.Assignee != null && assignee != _bug.Assignee)
        {
            SendEmailToAssignee("Don't forget to help the new employee!");
        }

        _bug.Assignee = assignee;

        SendEmailToAssignee("You own it.");
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
    }

    public BugWorkflow.State State { get; set; }
    public string Title { get; set; }
    public string Assignee { get; set; }
}