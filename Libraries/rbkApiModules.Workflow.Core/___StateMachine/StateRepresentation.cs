using System;
using System.Collections.Generic;
using System.Linq;

namespace Stateless;

internal partial class StateRepresentation<TState, TTrigger>
{
    readonly TState _state;

    internal IDictionary<TTrigger, ICollection<TriggerBehaviour<TState, TTrigger>>> TriggerBehaviours { get; } = new Dictionary<TTrigger, ICollection<TriggerBehaviour<TState, TTrigger>>>();
    internal ICollection<EntryActionBehavior<TState, TTrigger>> EntryActions { get; } = new List<EntryActionBehavior<TState, TTrigger>>();
    internal ICollection<ExitActionBehavior<TState, TTrigger>> ExitActions { get; } = new List<ExitActionBehavior<TState, TTrigger>>();
    internal ICollection<ActivateActionBehaviour<TState, TTrigger>> ActivateActions { get; } = new List<ActivateActionBehaviour<TState, TTrigger>>();
    internal ICollection<DeactivateActionBehaviour<TState, TTrigger>> DeactivateActions { get; } = new List<DeactivateActionBehaviour<TState, TTrigger>>();

    StateRepresentation<TState, TTrigger> _superstate; // null

    readonly ICollection<StateRepresentation<TState, TTrigger>> _substates = new List<StateRepresentation<TState, TTrigger>>();
    public TState InitialTransitionTarget { get; private set; } = default;

    public StateRepresentation(TState state)
    {
        _state = state;
    }

    internal ICollection<StateRepresentation<TState, TTrigger>> GetSubstates()
    {
        return _substates;
    }

    public bool CanHandle(TTrigger trigger, params object[] args)
    {
        return TryFindHandler(trigger, args, out TriggerBehaviourResult<TState, TTrigger> _);
    }

    public bool CanHandle(TTrigger trigger, object[] args, out ICollection<string> unmetGuards)
    {
        bool handlerFound = TryFindHandler(trigger, args, out TriggerBehaviourResult<TState, TTrigger> result);
        unmetGuards = result?.UnmetGuardConditions;
        return handlerFound;
    }

    public bool TryFindHandler(TTrigger trigger, object[] args, out TriggerBehaviourResult<TState, TTrigger> handler)
    {
        TriggerBehaviourResult<TState, TTrigger> superStateHandler = null;

        bool handlerFound = (TryFindLocalHandler(trigger, args, out TriggerBehaviourResult<TState, TTrigger> localHandler) ||
                            (Superstate != null && Superstate.TryFindHandler(trigger, args, out superStateHandler)));

        // If no handler for super state, replace by local handler (see issue #398)
        handler = superStateHandler ?? localHandler;

        return handlerFound;
    }

    private bool TryFindLocalHandler(TTrigger trigger, object[] args, out TriggerBehaviourResult<TState, TTrigger> handlerResult)
    {
        // Get list of candidate trigger handlers
        if (!TriggerBehaviours.TryGetValue(trigger, out ICollection<TriggerBehaviour<TState, TTrigger>> possible))
        {
            handlerResult = null;
            return false;
        }

        // Guard functions are executed here
        var actual = possible
            .Select(h => new TriggerBehaviourResult<TState, TTrigger>(h, h.UnmetGuardConditions(args)))
            .ToArray();

        // Find a handler for the trigger
        handlerResult = TryFindLocalHandlerResult(trigger, actual)
            ?? TryFindLocalHandlerResultWithUnmetGuardConditions(actual);

        if (handlerResult == null)
            return false;

        return !handlerResult.UnmetGuardConditions.Any();
    }

    private TriggerBehaviourResult<TState, TTrigger> TryFindLocalHandlerResult(TTrigger trigger, IEnumerable<TriggerBehaviourResult<TState, TTrigger>> results)
    {
        var actual = results
            .Where(r => !r.UnmetGuardConditions.Any())
            .ToList();

        if (actual.Count <= 1)
            return actual.FirstOrDefault();

        var message = string.Format("Multiple permitted exit transitions are configured from state '{1}' for trigger '{0}'. Guard clauses must be mutually exclusive.", trigger, _state);
        throw new InvalidOperationException(message);
    }

    private static TriggerBehaviourResult<TState, TTrigger> TryFindLocalHandlerResultWithUnmetGuardConditions(IEnumerable<TriggerBehaviourResult<TState, TTrigger>> results)
    {
        var result = results.FirstOrDefault(r => r.UnmetGuardConditions.Any());

        if (result != null)
        {
            var unmetConditions = results.Where(r => r.UnmetGuardConditions.Any())
                                         .SelectMany(r => r.UnmetGuardConditions);

            // Add other unmet conditions to first result
            foreach (var condition in unmetConditions)
            {
                if (!result.UnmetGuardConditions.Contains(condition))
                {
                    result.UnmetGuardConditions.Add(condition);
                }
            }
        }

        return result;
    }

    public void AddActivateAction(Action action, Reflection.InvocationInfo activateActionDescription)
    {
        ActivateActions.Add(new ActivateActionBehaviour<TState, TTrigger>.Sync(_state, action, activateActionDescription));
    }

    public void AddDeactivateAction(Action action, Reflection.InvocationInfo deactivateActionDescription)
    {
        DeactivateActions.Add(new DeactivateActionBehaviour<TState, TTrigger>.Sync(_state, action, deactivateActionDescription));
    }

    public void AddEntryAction(TTrigger trigger, Action<Transition<TState, TTrigger>, object[]> action, Reflection.InvocationInfo entryActionDescription)
    {
        EntryActions.Add(new EntryActionBehavior<TState, TTrigger>.SyncFrom<TTrigger>(trigger, action, entryActionDescription));
    }

    public void AddEntryAction(Action<Transition<TState, TTrigger>, object[]> action, Reflection.InvocationInfo entryActionDescription)
    {
        EntryActions.Add(new EntryActionBehavior<TState, TTrigger>.Sync(action, entryActionDescription));
    }

    public void AddExitAction(Action<Transition<TState, TTrigger>, object[]> action, Reflection.InvocationInfo exitActionDescription)
    {
        ExitActions.Add(new ExitActionBehavior<TState, TTrigger>.Sync(action, exitActionDescription));
    }

    public void AddExitAction(Action<object[]> action, Reflection.InvocationInfo exitActionDescription)
    {
        ExitActions.Add(new ExitActionBehavior<TState, TTrigger>.Sync((t, args) => action(args), exitActionDescription));
    }

    public void Activate()
    {
        if (_superstate != null)
            _superstate.Activate();

        ExecuteActivationActions();
    }

    public void Deactivate()
    {
        ExecuteDeactivationActions();

        if (_superstate != null)
            _superstate.Deactivate();
    }

    void ExecuteActivationActions()
    {
        foreach (var action in ActivateActions)
            action.Execute();
    }

    void ExecuteDeactivationActions()
    {
        foreach (var action in DeactivateActions)
            action.Execute();
    }

    public void Enter(Transition<TState, TTrigger> transition, params object[] entryArgs)
    {
        if (transition.IsReentry)
        {
            ExecuteEntryActions(transition, entryArgs);
        }
        else if (!Includes(transition.Source))
        {
            if (_superstate != null && !(transition is InitialTransition<TState, TTrigger>))
                _superstate.Enter(transition, entryArgs);

            ExecuteEntryActions(transition, entryArgs);
        }
    }

    public Transition<TState, TTrigger> Exit(Transition<TState, TTrigger> transition)
    {
        if (transition.IsReentry)
        {
            ExecuteExitActions(transition);
        }
        else if (!Includes(transition.Destination))
        {
            ExecuteExitActions(transition);

            // Must check if there is a superstate, and if we are leaving that superstate
            if (_superstate != null)
            {
                // Check if destination is within the state list
                if (IsIncludedIn(transition.Destination))
                {
                    // Destination state is within the list, exit first superstate only if it is NOT the the first
                    if (!_superstate.UnderlyingState.Equals(transition.Destination))
                    {
                        return _superstate.Exit(transition);
                    }
                }
                else
                {
                    // Exit the superstate as well
                    return _superstate.Exit(transition);
                }
            }
        }
        return transition;
    }

    void ExecuteEntryActions(Transition<TState, TTrigger> transition, object[] entryArgs)
    {
        foreach (var action in EntryActions)
            action.Execute(transition, entryArgs);
    }

    void ExecuteExitActions(Transition<TState, TTrigger> transition)
    {
        foreach (var action in ExitActions)
            action.Execute(transition);
    }
    internal void InternalAction(Transition<TState, TTrigger> transition, object[] args)
    {
        InternalTriggerBehaviour<TState, TTrigger>.Sync internalTransition = null;

        // Look for actions in superstate(s) recursivly until we hit the topmost superstate, or we actually find some trigger handlers.
        StateRepresentation<TState, TTrigger> aStateRep = this;
        while (aStateRep != null)
        {
            if (aStateRep.TryFindLocalHandler(transition.Trigger, args, out TriggerBehaviourResult<TState, TTrigger> result))
            {
                // Trigger handler found in this state
                if (result.Handler is InternalTriggerBehaviour<TState, TTrigger>.Async)
                    throw new InvalidOperationException("Running Async internal actions in synchronous mode is not allowed");

                internalTransition = result.Handler as InternalTriggerBehaviour<TState, TTrigger>.Sync;
                break;
            }
            // Try to look for trigger handlers in superstate (if it exists)
            aStateRep = aStateRep._superstate;
        }

        // Execute internal transition event handler
        if (internalTransition == null) throw new ArgumentNullException("The configuration is incorrect, no action assigned to this internal transition.");
        internalTransition.InternalAction(transition, args);
    }
    public void AddTriggerBehaviour(TriggerBehaviour<TState, TTrigger> triggerBehaviour)
    {
        if (!TriggerBehaviours.TryGetValue(triggerBehaviour.Trigger, out ICollection<TriggerBehaviour<TState, TTrigger>> allowed))
        {
            allowed = new List<TriggerBehaviour<TState, TTrigger>>();
            TriggerBehaviours.Add(triggerBehaviour.Trigger, allowed);
        }
        allowed.Add(triggerBehaviour);
    }

    public StateRepresentation<TState, TTrigger> Superstate
    {
        get
        {
            return _superstate;
        }
        set
        {
            _superstate = value;
        }
    }

    public TState UnderlyingState
    {
        get
        {
            return _state;
        }
    }

    public void AddSubstate(StateRepresentation<TState, TTrigger> substate)
    {
        _substates.Add(substate);
    }

    /// <summary>
    /// Checks if the state is in the set of this state or its sub-states
    /// </summary>
    /// <param name="state">The state to check</param>
    /// <returns>True if included</returns>
    public bool Includes(TState state)
    {
        return _state.Equals(state) || _substates.Any(s => s.Includes(state));
    }

    /// <summary>
    /// Checks if the state is in the set of this state or a super-state
    /// </summary>
    /// <param name="state">The state to check</param>
    /// <returns>True if included</returns>
    public bool IsIncludedIn(TState state)
    {
        return
            _state.Equals(state) ||
            (_superstate != null && _superstate.IsIncludedIn(state));
    }

    public IEnumerable<TTrigger> PermittedTriggers
    {
        get
        {
            return GetPermittedTriggers();
        }
    }

    public IEnumerable<TTrigger> GetPermittedTriggers(params object[] args)
    {
        var result = TriggerBehaviours
            .Where(t => t.Value.Any(a => !a.UnmetGuardConditions(args).Any()))
            .Select(t => t.Key);

        if (Superstate != null)
            result = result.Union(Superstate.GetPermittedTriggers(args));

        return result;
    }

    internal void SetInitialTransition(TState state)
    {
        InitialTransitionTarget = state;
        HasInitialTransition = true;
    }
    public bool HasInitialTransition { get; private set; }



































    public void AddActivateAction(Func<Task> action, Reflection.InvocationInfo activateActionDescription)
    {
        ActivateActions.Add(new ActivateActionBehaviour<TState, TTrigger>.Async(_state, action, activateActionDescription));
    }

    public void AddDeactivateAction(Func<Task> action, Reflection.InvocationInfo deactivateActionDescription)
    {
        DeactivateActions.Add(new DeactivateActionBehaviour<TState, TTrigger>.Async(_state, action, deactivateActionDescription));
    }

    public void AddEntryAction(TTrigger trigger, Func<Transition<TState, TTrigger>, object[], Task> action, Reflection.InvocationInfo entryActionDescription)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        EntryActions.Add(
            new EntryActionBehavior<TState, TTrigger>.Async((t, args) =>
            {
                if (t.Trigger.Equals(trigger))
                    return action(t, args);

                return TaskResult.Done;
            },
            entryActionDescription));
    }

    public void AddEntryAction(Func<Transition<TState, TTrigger>, object[], Task> action, Reflection.InvocationInfo entryActionDescription)
    {
        EntryActions.Add(
            new EntryActionBehavior<TState, TTrigger>.Async(
                action,
                entryActionDescription));
    }

    public void AddExitAction(Func<Transition<TState, TTrigger>, object[], Task> action, Reflection.InvocationInfo exitActionDescription)
    {
        ExitActions.Add(new ExitActionBehavior<TState, TTrigger>.Async(action, exitActionDescription));
    }

    public async Task ActivateAsync()
    {
        if (_superstate != null)
            await _superstate.ActivateAsync().ConfigureAwait(false);

        await ExecuteActivationActionsAsync().ConfigureAwait(false);
    }

    public async Task DeactivateAsync()
    {
        await ExecuteDeactivationActionsAsync().ConfigureAwait(false);

        if (_superstate != null)
            await _superstate.DeactivateAsync().ConfigureAwait(false);
    }

    async Task ExecuteActivationActionsAsync()
    {
        foreach (var action in ActivateActions)
            await action.ExecuteAsync().ConfigureAwait(false);
    }

    async Task ExecuteDeactivationActionsAsync()
    {
        foreach (var action in DeactivateActions)
            await action.ExecuteAsync().ConfigureAwait(false);
    }


    public async Task EnterAsync(Transition<TState, TTrigger> transition, params object[] entryArgs)
    {
        if (transition.IsReentry)
        {
            await ExecuteEntryActionsAsync(transition, entryArgs).ConfigureAwait(false);
        }
        else if (!Includes(transition.Source))
        {
            if (_superstate != null && !(transition is InitialTransition<TState, TTrigger>))
                await _superstate.EnterAsync(transition, entryArgs).ConfigureAwait(false);

            await ExecuteEntryActionsAsync(transition, entryArgs).ConfigureAwait(false);
        }
    }

    public async Task<Transition<TState, TTrigger>> ExitAsync(Transition<TState, TTrigger> transition)
    {
        if (transition.IsReentry)
        {
            await ExecuteExitActionsAsync(transition).ConfigureAwait(false);
        }
        else if (!Includes(transition.Destination))
        {
            await ExecuteExitActionsAsync(transition).ConfigureAwait(false);

            // Must check if there is a superstate, and if we are leaving that superstate
            if (_superstate != null)
            {
                // Check if destination is within the state list
                if (IsIncludedIn(transition.Destination))
                {
                    // Destination state is within the list, exit first superstate only if it is NOT the first
                    if (!_superstate.UnderlyingState.Equals(transition.Destination))
                    {
                        return await _superstate.ExitAsync(transition).ConfigureAwait(false);
                    }
                }
                else
                {
                    // Exit the superstate as well
                    return await _superstate.ExitAsync(transition).ConfigureAwait(false);
                }
            }
        }
        return transition;
    }

    async Task ExecuteEntryActionsAsync(Transition<TState, TTrigger> transition, object[] entryArgs)
    {
        foreach (var action in EntryActions)
            await action.ExecuteAsync(transition, entryArgs).ConfigureAwait(false);
    }

    async Task ExecuteExitActionsAsync(Transition<TState, TTrigger> transition)
    {
        foreach (var action in ExitActions)
            await action.ExecuteAsync(transition).ConfigureAwait(false);
    }
}

