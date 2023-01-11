using System;
using System.Collections.Generic;
using System.Linq;

namespace Stateless;

internal class TransitionGuard
{
    internal IList<GuardCondition> Conditions { get; }

    public static readonly TransitionGuard Empty = new TransitionGuard(new NamedGuard[0]);

    public static Func<object[], bool> ToPackedGuard(Func<object[], bool> guard)
    {
        return args => guard(args);
    } 

    internal TransitionGuard(NamedGuard[] guards)
    {
        Conditions = guards
            .Select(g => new GuardCondition(g.Guard, Reflection.InvocationInfo.Create(g.Guard, g.Description)))
            .ToList();
    }

    internal TransitionGuard(NamedGuard guard)
    {
        Conditions = new List<GuardCondition> { new GuardCondition(guard.Guard, Reflection.InvocationInfo.Create(guard.Guard, guard.Description)) };
    }

    internal TransitionGuard(Func<object[], bool> guard, string description = null)
    {
        Conditions = new List<GuardCondition>
            {
                new GuardCondition(guard, Reflection.InvocationInfo.Create(guard, description))
            };
    }

    /// <summary>
    /// Guards is the list of the guard functions for all guard conditions for this transition
    /// </summary>
    internal ICollection<Func<object[], bool>> Guards => Conditions.Select(g => g.Guard).ToList();

    /// <summary>
    /// GuardConditionsMet is true if all of the guard functions return true
    /// or if there are no guard functions
    /// </summary>
    public bool GuardConditionsMet(object[] args)
    {
        return Conditions.All(c => c.Guard == null || c.Guard(args));
    }

    /// <summary>
    /// UnmetGuardConditions is a list of the descriptions of all guard conditions
    /// whose guard function returns false
    /// </summary>
    public ICollection<string> UnmetGuardConditions(object[] args)
    {
        return Conditions
            .Where(c => !c.Guard(args))
            .Select(c => c.Description)
            .ToList();
    }
}