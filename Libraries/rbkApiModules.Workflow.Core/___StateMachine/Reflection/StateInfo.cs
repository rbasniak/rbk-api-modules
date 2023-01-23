namespace Stateless.Reflection;

/// <summary>
/// Describes an internal StateRepresentation through the reflection API.
/// </summary>
public class StateInfo<TState, TTrigger>
{
    internal static StateInfo<TState, TTrigger> CreateStateInfo<TState, TTrigger>(StateRepresentation<TState, TTrigger> stateRepresentation)
    {
        if (stateRepresentation == null)
            throw new ArgumentException(nameof(stateRepresentation));

        var ignoredTriggers = new List<IgnoredTransitionInfo<TState, TTrigger>>();

        // stateRepresentation.TriggerBehaviours maps from TTrigger to ICollection<TriggerBehaviour>
        foreach (var triggerBehaviours in stateRepresentation.TriggerBehaviours)
        {
            foreach (var item in triggerBehaviours.Value)
            {
                if (item is IgnoredTriggerBehaviour<TState, TTrigger> behaviour)
                {
                    ignoredTriggers.Add(IgnoredTransitionInfo<TState, TTrigger>.Create(behaviour));
                }
            }
        }

        return new StateInfo<TState, TTrigger>(stateRepresentation.UnderlyingState, ignoredTriggers,
            stateRepresentation.EntryActions.Select(e => ActionInfo.Create(e)).ToList(),
            stateRepresentation.ExitActions.Select(e => e.Description).ToList());
    }

    private StateInfo(
    object underlyingState,
        IEnumerable<IgnoredTransitionInfo<TState, TTrigger>> ignoredTriggers,
        IEnumerable<ActionInfo> entryActions,
        IEnumerable<InvocationInfo> exitActions)
    {
        UnderlyingState = underlyingState;
        IgnoredTriggers = ignoredTriggers ?? throw new ArgumentNullException(nameof(ignoredTriggers));
        EntryActions = entryActions;
        ExitActions = exitActions;
    }

    /// <summary>
    /// The instance or value this state represents.
    /// </summary>
    public object UnderlyingState { get; }

    /// <summary>
    /// Actions that are defined to be executed on state-entry.
    /// </summary>
    public IEnumerable<ActionInfo> EntryActions { get; private set; }

    /// <summary>
    /// Actions that are defined to be executed on activation.
    /// </summary>
    public IEnumerable<InvocationInfo> ActivateActions { get; private set; }

    /// <summary>
    /// Actions that are defined to be executed on deactivation.
    /// </summary>
    public IEnumerable<InvocationInfo> DeactivateActions { get; private set; }

    /// <summary>
    /// Actions that are defined to be exectuted on state-exit.
    /// </summary>
    public IEnumerable<InvocationInfo> ExitActions { get; private set; }

    /// <summary> 
    /// Transitions defined for this state.
    /// </summary>
    public IEnumerable<TransitionInfo> Transitions { get { return FixedTransitions; } }

    /// <summary>
    /// Transitions defined for this state.
    /// </summary>
    public IEnumerable<FixedTransitionInfo<TState, TTrigger>> FixedTransitions { get; private set; }
     
    /// <summary>
    /// Triggers ignored for this state.
    /// </summary>
    public IEnumerable<IgnoredTransitionInfo<TState, TTrigger>> IgnoredTriggers { get; private set; }

    internal static void AddRelationships(StateInfo<TState, TTrigger> info, StateRepresentation<TState, TTrigger> stateRepresentation, Func<TState, StateInfo<TState, TTrigger>> lookupState)
    {
        if (lookupState == null) throw new ArgumentNullException(nameof(lookupState));

        var fixedTransitions = new List<FixedTransitionInfo<TState, TTrigger>>();

        foreach (var triggerBehaviours in stateRepresentation.TriggerBehaviours)
        {
            // First add all the deterministic transitions
            foreach (var item in triggerBehaviours.Value.Where(behaviour => (behaviour is TransitioningTriggerBehaviour<TState, TTrigger>)))
            {
                var destinationInfo = lookupState(((TransitioningTriggerBehaviour<TState, TTrigger>)item).Destination);
                fixedTransitions.Add(FixedTransitionInfo<TState, TTrigger>.Create(item, destinationInfo));
            }
            foreach (var item in triggerBehaviours.Value.Where(behaviour => (behaviour is ReentryTriggerBehaviour<TState, TTrigger>)))
            {
                var destinationInfo = lookupState(((ReentryTriggerBehaviour<TState, TTrigger>)item).Destination);
                fixedTransitions.Add(FixedTransitionInfo<TState, TTrigger>.Create(item, destinationInfo));
            }
        }

        info.AddRelationships(fixedTransitions);
    }

    private void AddRelationships(IEnumerable<FixedTransitionInfo<TState, TTrigger>> transitions)
    {
        FixedTransitions = transitions ?? throw new ArgumentNullException(nameof(transitions));
    }

    /// <summary>
    /// Passes through to the value's ToString.
    /// </summary>
    public override string ToString()
    {
        return UnderlyingState?.ToString() ?? "<null>";
    }
}
