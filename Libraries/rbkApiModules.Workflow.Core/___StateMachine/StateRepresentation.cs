namespace Stateless;

internal partial class StateRepresentation<TState, TTrigger>
{
    readonly TState _state;

    internal IDictionary<TTrigger, ICollection<TriggerBehaviour<TState, TTrigger>>> TriggerBehaviours { get; } = new Dictionary<TTrigger, ICollection<TriggerBehaviour<TState, TTrigger>>>();
    internal ICollection<EntryActionBehavior<TState, TTrigger>> EntryActions { get; } = new List<EntryActionBehavior<TState, TTrigger>>();
    internal ICollection<ExitActionBehavior<TState, TTrigger>> ExitActions { get; } = new List<ExitActionBehavior<TState, TTrigger>>();

    public StateRepresentation(TState state)
    {
        _state = state;
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

        bool handlerFound = TryFindLocalHandler(trigger, args, out TriggerBehaviourResult<TState, TTrigger> localHandler);

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

    public void AddEntryAction(TTrigger trigger, Action<Transition<TState, TTrigger>> action, Reflection.InvocationInfo entryActionDescription)
    {
        EntryActions.Add(new EntryActionBehavior<TState, TTrigger>.SyncFrom<TTrigger>(trigger, action, entryActionDescription));
    }

    public void AddEntryAction(Action<Transition<TState, TTrigger>> action, Reflection.InvocationInfo entryActionDescription)
    {
        EntryActions.Add(new EntryActionBehavior<TState, TTrigger>.Sync(action, entryActionDescription));
    }

    public void AddExitAction(Action<Transition<TState, TTrigger>> action, Reflection.InvocationInfo exitActionDescription)
    {
        ExitActions.Add(new ExitActionBehavior<TState, TTrigger>.Sync(action, exitActionDescription));
    }

    public void Enter(Transition<TState, TTrigger> transition, params object[] entryArgs)
    {
        if (transition.IsReentry)
        {
            ExecuteEntryActions(transition);
        }
        else if (!Includes(transition.Source))
        {
            ExecuteEntryActions(transition);
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
        }
        return transition;
    }

    void ExecuteEntryActions(Transition<TState, TTrigger> transition)
    {
        foreach (var action in EntryActions)
            action.Execute(transition);
    }

    void ExecuteExitActions(Transition<TState, TTrigger> transition)
    {
        foreach (var action in ExitActions)
            action.Execute(transition);
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

    public TState UnderlyingState
    {
        get
        {
            return _state;
        }
    } 

    /// <summary>
    /// Checks if the state is in the set of this state or its sub-states
    /// </summary>
    /// <param name="state">The state to check</param>
    /// <returns>True if included</returns>
    public bool Includes(TState state)
    {
        return _state.Equals(state);
    }

    /// <summary>
    /// Checks if the state is in the set of this state or a super-state
    /// </summary>
    /// <param name="state">The state to check</param>
    /// <returns>True if included</returns>
    public bool IsIncludedIn(TState state)
    {
        return _state.Equals(state);
    }

    public TTrigger[] PermittedTriggers
    {
        get
        {
            return GetPermittedTriggers();
        }
    }

    public TTrigger[] GetPermittedTriggers(params object[] args)
    {
        var result = TriggerBehaviours
            .Where(t => t.Value.Any(a => !a.UnmetGuardConditions(args).Any()))
            .Select(t => t.Key);

        return result.ToArray();
    }
































    public void AddEntryAction(TTrigger trigger, Func<Transition<TState, TTrigger>, Task> action, Reflection.InvocationInfo entryActionDescription)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        EntryActions.Add(
            new EntryActionBehavior<TState, TTrigger>.Async(t =>
            {
                if (t.Trigger.Equals(trigger))
                    return action(t);

                return TaskResult.Done;
            },
            entryActionDescription));
    }

    public void AddEntryAction(Func<Transition<TState, TTrigger>, Task> action, Reflection.InvocationInfo entryActionDescription)
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


    public async Task EnterAsync(Transition<TState, TTrigger> transition)
    {
        if (transition.IsReentry)
        {
            await ExecuteEntryActionsAsync(transition).ConfigureAwait(false);
        }
        else if (!Includes(transition.Source))
        {
            await ExecuteEntryActionsAsync(transition).ConfigureAwait(false);
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
        }
        return transition;
    }

    async Task ExecuteEntryActionsAsync(Transition<TState, TTrigger> transition)
    {
        foreach (var action in EntryActions)
            await action.ExecuteAsync(transition).ConfigureAwait(false);
    }

    async Task ExecuteExitActionsAsync(Transition<TState, TTrigger> transition)
    {
        foreach (var action in ExitActions)
            await action.ExecuteAsync(transition).ConfigureAwait(false);
    }
}

