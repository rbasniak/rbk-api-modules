using System.Collections.Generic;
using System.Linq;

namespace Stateless.Reflection;

/// <summary>
/// Describes a transition that can be initiated from a trigger.
/// </summary>
public class FixedTransitionInfo<TState, TTrigger> : TransitionInfo
{
    internal static FixedTransitionInfo<TState, TTrigger> Create<TState, TTrigger>(TriggerBehaviour<TState, TTrigger> behaviour, StateInfo<TState, TTrigger> destinationStateInfo)
    {
        var transition = new FixedTransitionInfo<TState, TTrigger>
        {
            Trigger = new TriggerInfo(behaviour.Trigger),
            DestinationState = destinationStateInfo,
            GuardConditionsMethodDescriptions = (behaviour.Guard == null)
                ? new List<InvocationInfo>() : behaviour.Guard.Conditions.Select(c => c.MethodDescription)
        };

        return transition;
    }

    private FixedTransitionInfo() { }

    /// <summary>
    /// The state that will be transitioned into on activation.
    /// </summary>
    public StateInfo<TState, TTrigger> DestinationState { get; private set; }
}