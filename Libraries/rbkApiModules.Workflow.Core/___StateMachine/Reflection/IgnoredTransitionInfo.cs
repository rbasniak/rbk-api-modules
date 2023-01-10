using System.Collections.Generic;
using System.Linq;

namespace Stateless.Reflection;

/// <summary>
/// Describes a trigger that is "ignored" (stays in the same state)
/// </summary>
public class IgnoredTransitionInfo<TState, TTrigger> : TransitionInfo
{
    internal static IgnoredTransitionInfo<TState, TTrigger> Create(IgnoredTriggerBehaviour<TState, TTrigger> behaviour)
    {
        var transition = new IgnoredTransitionInfo<TState, TTrigger>
        {
            Trigger = new TriggerInfo(behaviour.Trigger),
            GuardConditionsMethodDescriptions = (behaviour.Guard == null)
                ? new List<InvocationInfo>() : behaviour.Guard.Conditions.Select(c => c.MethodDescription)
        };

        return transition;
    }

    private IgnoredTransitionInfo() { }
}