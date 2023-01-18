﻿namespace Stateless;

/// <summary>
/// The configuration for a single state value.
/// </summary>
public partial class StateConfiguration<TState, TTrigger>
{
    private readonly StateMachine<TState, TTrigger> _machine;
    readonly StateRepresentation<TState, TTrigger> _representation;
    readonly Func<TState, StateRepresentation<TState, TTrigger>> _lookup;

    internal StateConfiguration(StateMachine<TState, TTrigger> machine, StateRepresentation<TState, TTrigger> representation, Func<TState, StateRepresentation<TState, TTrigger>> lookup)
    {
        _machine = machine;
        _representation = representation;
        _lookup = lookup;
    }

    /// <summary>
    /// The state that is configured with this configuration.
    /// </summary>
    public TState State { get { return _representation.UnderlyingState; } }

    /// <summary>
    /// The machine that is configured with this configuration.
    /// </summary>
    public StateMachine<TState, TTrigger> Machine { get { return _machine; } }

    /// <summary>
    /// Accept the specified trigger and transition to the destination state.
    /// </summary>
    /// <param name="trigger">The accepted trigger.</param>
    /// <param name="destinationState">The state that the trigger will cause a
    /// transition to.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> Permit(TTrigger trigger, TState destinationState)
    {
        EnforceNotIdentityTransition(destinationState);
        return InternalPermit(trigger, destinationState);
    }

    /// <summary>
    /// Add an internal transition to the state machine. An internal action does not cause the Exit and Entry actions to be triggered, and does not change the state of the state machine
    /// </summary>
    public StateConfiguration<TState, TTrigger> InternalTransition(TTrigger trigger, Action internalAction)
    {
        return InternalTransitionIf(trigger, t => true, internalAction);

    }

    /// <summary>
    /// Add an internal transition to the state machine. An internal action does not cause the Exit and Entry actions to be triggered, and does not change the state of the state machine
    /// </summary>
    public StateConfiguration<TState, TTrigger> InternalTransition(TTrigger trigger, Action<Transition<TState, TTrigger>> internalAction)
    {
        return InternalTransitionIf(trigger, t => true, internalAction);
    }

    /// <summary>
    /// Add an internal transition to the state machine. An internal action does not cause the Exit and Entry actions to be triggered, and does not change the state of the state machine
    /// </summary>
    public StateConfiguration<TState, TTrigger> InternalTransitionIf(TTrigger trigger, Func<object[], bool> guard, Action<Transition<TState, TTrigger>> entryAction, string guardDescription = null)
    {
        if (entryAction == null) throw new ArgumentNullException(nameof(entryAction));

        _representation.AddTriggerBehaviour(new InternalTriggerBehaviour<TState, TTrigger>.Sync(trigger, guard, t => entryAction(t), guardDescription));
        return this;
    }

    /// <summary>
    /// Add an internal transition to the state machine. An internal action does not cause the Exit and Entry actions to be triggered, and does not change the state of the state machine
    /// </summary>
    public StateConfiguration<TState, TTrigger> InternalTransitionIf(TTrigger trigger, Func<object[], bool> guard, Action internalAction, string guardDescription = null)
    {
        if (internalAction == null) throw new ArgumentNullException(nameof(internalAction));

        _representation.AddTriggerBehaviour(new InternalTriggerBehaviour<TState, TTrigger>.Sync(trigger, guard, t => internalAction(), guardDescription));
        return this;
    } 

    /// <summary>
    /// Accept the specified trigger and transition to the destination state.
    /// </summary>
    /// <param name="trigger">The accepted trigger.</param>
    /// <param name="guards">Functions and their descriptions that must return true in order for the
    /// trigger to be accepted.</param>
    /// <param name="destinationState">State of the destination.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> PermitIf(TTrigger trigger, TState destinationState, params GuardDefinition[] guards)
    {
        EnforceNotIdentityTransition(destinationState);

        return InternalPermitIf(
            trigger,
            destinationState,
            new TransitionGuard(guards));
    }

    /// <summary>
    /// Accept the specified trigger, execute exit actions and re-execute entry actions.
    /// Reentry behaves as though the configured state transitions to an identical sibling state.
    /// </summary>
    /// <param name="trigger">The accepted trigger.</param>
    /// <returns>The receiver.</returns>
    /// <remarks>
    /// Applies to the current state only. Will not re-execute superstate actions, or
    /// cause actions to execute transitioning between super- and sub-states.
    /// </remarks>
    public StateConfiguration<TState, TTrigger> PermitReentry(TTrigger trigger)
    {
        return InternalPermitReentryIf(trigger, _representation.UnderlyingState, null);
    }

    /// <summary>
    /// Accept the specified trigger, execute exit actions and re-execute entry actions.
    /// Reentry behaves as though the configured state transitions to an identical sibling state.
    /// </summary>
    /// <param name="trigger">The accepted trigger.</param>
    /// <param name="guards">Functions and their descriptions that must return true in order for the
    /// trigger to be accepted.</param>
    /// <returns>The receiver.</returns>
    /// <remarks>
    /// Applies to the current state only. Will not re-execute superstate actions, or
    /// cause actions to execute transitioning between super- and sub-states.
    /// </remarks>
    public StateConfiguration<TState, TTrigger> PermitReentryIf(TTrigger trigger, params GuardDefinition[] guards)
    {
        return InternalPermitReentryIf(
            trigger,
            _representation.UnderlyingState,
            new TransitionGuard(guards));
    }

    /// <summary>
    /// Ignore the specified trigger when in the configured state, if the guard
    /// returns true..
    /// </summary>
    /// <param name="trigger">The trigger to ignore.</param>
    /// <param name="guards">Functions and their descriptions that must return true in order for the
    /// trigger to be ignored.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> IgnoreIf(TTrigger trigger, params GuardDefinition[] guards)
    {
        _representation.AddTriggerBehaviour(
            new IgnoredTriggerBehaviour<TState, TTrigger>(
                trigger,
                new TransitionGuard(guards)));
        return this;
    }

    // TODO: REMOVE ACTIVATION
    ///// <summary>
    ///// Specify an action that will execute when activating
    ///// the configured state.
    ///// </summary>
    ///// <param name="activateAction">Action to execute.</param>
    ///// <param name="activateActionDescription">Action description.</param>
    ///// <returns>The receiver.</returns>
    //public StateConfiguration<TState, TTrigger> OnActivate(Action activateAction, string activateActionDescription = null)
    //{
    //    _representation.AddActivateAction(
    //        activateAction,
    //        Reflection.InvocationInfo.Create(activateAction, activateActionDescription));
    //    return this;
    //}

    // TODO: REMOVE ACTIVATION
    ///// <summary>
    ///// Specify an action that will execute when deactivating
    ///// the configured state.
    ///// </summary>
    ///// <param name="deactivateAction">Action to execute.</param>
    ///// <param name="deactivateActionDescription">Action description.</param>
    ///// <returns>The receiver.</returns>
    //public StateConfiguration<TState, TTrigger> OnDeactivate(Action deactivateAction, string deactivateActionDescription = null)
    //{
    //    _representation.AddDeactivateAction(
    //        deactivateAction,
    //        Reflection.InvocationInfo.Create(deactivateAction, deactivateActionDescription));
    //    return this;
    //}

    /// <summary>
    /// Specify an action that will execute when transitioning into
    /// the configured state.
    /// </summary>
    /// <param name="entryAction">Action to execute.</param>
    /// <param name="entryActionDescription">Action description.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> OnEntry(Action entryAction, string entryActionDescription = null)
    {
        if (entryAction == null) throw new ArgumentNullException(nameof(entryAction));

        _representation.AddEntryAction(
            t => entryAction(),
            Reflection.InvocationInfo.Create(entryAction, entryActionDescription));
        return this;

    }

    /// <summary>
    /// Specify an action that will execute when transitioning into
    /// the configured state.
    /// </summary>
    /// <param name="entryAction">Action to execute, providing details of the transition.</param>
    /// <param name="entryActionDescription">Action description.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> OnEntry(Action<Transition<TState, TTrigger>> entryAction, string entryActionDescription = null)
    {
        if (entryAction == null) throw new ArgumentNullException(nameof(entryAction));

        _representation.AddEntryAction(
            t => entryAction(t),
            Reflection.InvocationInfo.Create(entryAction, entryActionDescription));
        return this;
    }

    /// <summary>
    /// Specify an action that will execute when transitioning into
    /// the configured state.
    /// </summary>
    /// <param name="entryAction">Action to execute.</param>
    /// <param name="trigger">The trigger by which the state must be entered in order for the action to execute.</param>
    /// <param name="entryActionDescription">Action description.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> OnEntryFrom(TTrigger trigger, Action entryAction, string entryActionDescription = null)
    {
        if (entryAction == null) throw new ArgumentNullException(nameof(entryAction));

        _representation.AddEntryAction(
            trigger,
            t => entryAction(),
            Reflection.InvocationInfo.Create(entryAction, entryActionDescription));
        return this;

    }

    /// <summary>
    /// Specify an action that will execute when transitioning into
    /// the configured state.
    /// </summary>
    /// <param name="entryAction">Action to execute, providing details of the transition.</param>
    /// <param name="trigger">The trigger by which the state must be entered in order for the action to execute.</param>
    /// <param name="entryActionDescription">Action description.</param>
    public StateConfiguration<TState, TTrigger> OnEntryFrom(TTrigger trigger, Action<Transition<TState, TTrigger>> entryAction, string entryActionDescription = null)
    {
        if (entryAction == null) throw new ArgumentNullException(nameof(entryAction));

        _representation.AddEntryAction(
            trigger,
            t => entryAction(t),
            Reflection.InvocationInfo.Create(entryAction, entryActionDescription));
        return this;
    }  

    /// <summary>
    /// Specify an action that will execute when transitioning from
    /// the configured state.
    /// </summary>
    /// <param name="exitAction">Action to execute, providing details of the transition.</param>
    /// <param name="exitActionDescription">Action description.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> OnExit(Action exitAction, string exitActionDescription = null)
    {
        _representation.AddExitAction(
            t => exitAction(),
            Reflection.InvocationInfo.Create(exitAction, exitActionDescription));
        return this;
    }

    public StateConfiguration<TState, TTrigger> OnExit(Action<Transition<TState, TTrigger>> exitAction, string exitActionDescription = null)
    {
        _representation.AddExitAction(
            t => exitAction(t),
            Reflection.InvocationInfo.Create(exitAction, exitActionDescription));
        return this;
    } 

    void EnforceNotIdentityTransition(TState destination)
    {
        if (destination.Equals(_representation.UnderlyingState))
        {
            throw new ArgumentException("StateConfigurationResources.SelfTransitionsEitherIgnoredOrReentrant");
        }
    }

    StateConfiguration<TState, TTrigger> InternalPermit(TTrigger trigger, TState destinationState)
    {
        _representation.AddTriggerBehaviour(new TransitioningTriggerBehaviour<TState, TTrigger>(trigger, destinationState, null));
        return this;
    }

    StateConfiguration<TState, TTrigger> InternalPermitIf(TTrigger trigger, TState destinationState, TransitionGuard transitionGuard)
    {
        _representation.AddTriggerBehaviour(new TransitioningTriggerBehaviour<TState, TTrigger>(trigger, destinationState, transitionGuard));
        return this;
    }

    StateConfiguration<TState, TTrigger> InternalPermitReentryIf(TTrigger trigger, TState destinationState, TransitionGuard transitionGuard)
    {
        _representation.AddTriggerBehaviour(new ReentryTriggerBehaviour<TState, TTrigger>(trigger, destinationState, transitionGuard));
        return this;
    } 









    /// <summary>
    /// Add an internal transition to the state machine. An internal action does not cause the Exit and Entry actions to be triggered, and does not change the state of the state machine
    /// </summary>
    /// <param name="trigger"></param>
    /// <param name="guards">Function that must return true in order for the trigger to be accepted.</param>
    /// <param name="entryAction"></param>
    /// <returns></returns>
    public StateConfiguration<TState, TTrigger> InternalTransitionAsyncIf(TTrigger trigger, GuardDefinition[] guards, Func<object[], Transition<TState, TTrigger>, Task> entryAction)
    {
        _representation.AddTriggerBehaviour(new InternalTriggerBehaviour<TState, TTrigger>.Async(trigger, guards, (t, args) => entryAction(args, t)));
        return this;
    }

    /// <summary>
    /// Add an internal transition to the state machine. An internal action does not cause the Exit and Entry actions to be triggered, and does not change the state of the state machine
    /// </summary>
    /// <param name="trigger">The accepted trigger</param>
    /// <param name="guard">Function that must return true in order for the\r\n            /// trigger to be accepted.</param>
    /// <param name="internalAction">The asynchronous action performed by the internal transition</param>
    /// <returns></returns>
    public StateConfiguration<TState, TTrigger> InternalTransitionAsyncIf(TTrigger trigger, GuardDefinition[] guards, Func<object[], Task> internalAction)
    {
        if (internalAction == null) throw new ArgumentNullException(nameof(internalAction));

        _representation.AddTriggerBehaviour(new InternalTriggerBehaviour<TState, TTrigger>.Async(trigger, guards, (t, args) => internalAction(args)));
        return this;
    }

    /// <summary>
    /// Add an internal transition to the state machine. An internal action does not cause the Exit and Entry actions to be triggered, and does not change the state of the state machine
    /// </summary>
    /// <param name="trigger"></param>
    /// <param name="entryAction"></param>
    /// <returns></returns>
    public StateConfiguration<TState, TTrigger> InternalTransitionAsync(TTrigger trigger, Func<object[], Transition<TState, TTrigger>, Task> entryAction)
    {
        return InternalTransitionAsyncIf(trigger, new[] { new GuardDefinition(String.Empty, (args) => true) }, entryAction);
    }

    /// <summary>
    /// Add an internal transition to the state machine. An internal action does not cause the Exit and Entry actions to be triggered, and does not change the state of the state machine
    /// </summary>
    /// <param name="trigger">The accepted trigger</param>
    /// <param name="internalAction">The asynchronous action performed by the internal transition</param>
    /// <returns></returns>
    public StateConfiguration<TState, TTrigger> InternalTransitionAsync(TTrigger trigger, Func<object[], Task> internalAction)
    {
        return InternalTransitionAsyncIf(trigger, new[] { new GuardDefinition(String.Empty, (args) => true) }, internalAction);
    }

    // TODO: REMOVE ACTIVATION
    ///// <summary>
    ///// Specify an asynchronous action that will execute when activating
    ///// the configured state.
    ///// </summary>
    ///// <param name="activateAction">Action to execute.</param>
    ///// <param name="activateActionDescription">Action description.</param>
    ///// <returns>The receiver.</returns>
    //public StateConfiguration<TState, TTrigger> OnActivateAsync(Func<Task> activateAction, string activateActionDescription = null)
    //{
    //    _representation.AddActivateAction(
    //        activateAction,
    //        Reflection.InvocationInfo.Create(activateAction, activateActionDescription, Reflection.InvocationInfo.Timing.Asynchronous));
    //    return this;
    //}

    // TODO: REMOVE ACTIVATION
    ///// <summary>
    ///// Specify an asynchronous action that will execute when deactivating
    ///// the configured state.
    ///// </summary>
    ///// <param name="deactivateAction">Action to execute.</param>
    ///// <param name="deactivateActionDescription">Action description.</param>
    ///// <returns>The receiver.</returns>
    //public StateConfiguration<TState, TTrigger> OnDeactivateAsync(Func<Task> deactivateAction, string deactivateActionDescription = null)
    //{
    //    _representation.AddDeactivateAction(
    //        deactivateAction,
    //        Reflection.InvocationInfo.Create(deactivateAction, deactivateActionDescription, Reflection.InvocationInfo.Timing.Asynchronous));
    //    return this;
    //}

    /// <summary>
    /// Specify an asynchronous action that will execute when transitioning into
    /// the configured state.
    /// </summary>
    /// <param name="entryAction">Action to execute.</param>
    /// <param name="entryActionDescription">Action description.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> OnEntryAsync(Func<Task> entryAction, string entryActionDescription = null)
    {
        if (entryAction == null) throw new ArgumentNullException(nameof(entryAction));

        _representation.AddEntryAction(
            t => entryAction(),
            Reflection.InvocationInfo.Create(entryAction, entryActionDescription, Reflection.InvocationInfo.Timing.Asynchronous));
        return this;

    }

    /// <summary>
    /// Specify an asynchronous action that will execute when transitioning into
    /// the configured state.
    /// </summary>
    /// <param name="entryAction">Action to execute, providing details of the transition.</param>
    /// <param name="entryActionDescription">Action description.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> OnEntryAsync(Func<Transition<TState, TTrigger>, Task> entryAction, string entryActionDescription = null)
    {
        if (entryAction == null) throw new ArgumentNullException(nameof(entryAction));

        _representation.AddEntryAction(
            t => entryAction(t),
            Reflection.InvocationInfo.Create(entryAction, entryActionDescription, Reflection.InvocationInfo.Timing.Asynchronous));
        return this;
    }

    /// <summary>
    /// Specify an asynchronous action that will execute when transitioning into
    /// the configured state.
    /// </summary>
    /// <param name="entryAction">Action to execute.</param>
    /// <param name="trigger">The trigger by which the state must be entered in order for the action to execute.</param>
    /// <param name="entryActionDescription">Action description.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> OnEntryFromAsync(TTrigger trigger, Func<Task> entryAction, string entryActionDescription = null)
    {
        if (entryAction == null) throw new ArgumentNullException(nameof(entryAction));

        _representation.AddEntryAction(
            trigger,
            t => entryAction(),
            Reflection.InvocationInfo.Create(entryAction, entryActionDescription, Reflection.InvocationInfo.Timing.Asynchronous));
        return this;
    }

    /// <summary>
    /// Specify an asynchronous action that will execute when transitioning into
    /// the configured state.
    /// </summary>
    /// <param name="entryAction">Action to execute, providing details of the transition.</param>
    /// <param name="trigger">The trigger by which the state must be entered in order for the action to execute.</param>
    /// <param name="entryActionDescription">Action description.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> OnEntryFromAsync(TTrigger trigger, Func<Transition<TState, TTrigger>, Task> entryAction, string entryActionDescription = null)
    {
        if (entryAction == null) throw new ArgumentNullException(nameof(entryAction));

        _representation.AddEntryAction(
            trigger,
            t => entryAction(t),
            Reflection.InvocationInfo.Create(entryAction, entryActionDescription, Reflection.InvocationInfo.Timing.Asynchronous));
        return this;
    }

    /// <summary>
    /// Specify an asynchronous action that will execute when transitioning from
    /// the configured state.
    /// </summary>
    /// <param name="exitAction">Action to execute.</param>
    /// <param name="exitActionDescription">Action description.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> OnExitAsync(Func<Task> exitAction, string exitActionDescription = null)
    {
        if (exitAction == null) throw new ArgumentNullException(nameof(exitAction));

        _representation.AddExitAction(
            t => exitAction(),
            Reflection.InvocationInfo.Create(exitAction, exitActionDescription, Reflection.InvocationInfo.Timing.Asynchronous));
        return this;
    }

    /// <summary>
    /// Specify an asynchronous action that will execute when transitioning from
    /// the configured state.
    /// </summary>
    /// <param name="exitAction">Action to execute, providing details of the transition.</param>
    /// <param name="exitActionDescription">Action description.</param>
    /// <returns>The receiver.</returns>
    public StateConfiguration<TState, TTrigger> OnExitAsync(Func<Transition<TState, TTrigger>, object[], Task> exitAction, string exitActionDescription = null)
    {
        _representation.AddExitAction(
            exitAction,
            Reflection.InvocationInfo.Create(exitAction, exitActionDescription, Reflection.InvocationInfo.Timing.Asynchronous));
        return this;
    }

    public StateConfiguration<TState, TTrigger> OnExitAsync(Func<Transition<TState, TTrigger>, Task> exitAction, string exitActionDescription = null)
    {
        _representation.AddExitAction(
            (t, args) => exitAction(t),
            Reflection.InvocationInfo.Create(exitAction, exitActionDescription, Reflection.InvocationInfo.Timing.Asynchronous));
        return this;
    }
}

public class GuardDefinition
{
    public GuardDefinition(string description, Func<object[], bool> guard)
    {
        Guard = guard;
        Description = description;
    }

    public GuardDefinition(Func<object[], bool> guard)
    {
        Guard = guard;
        Description = String.Empty;
    }

    public string Description { get; }

    public Func<object[], bool> Guard { get; }
}