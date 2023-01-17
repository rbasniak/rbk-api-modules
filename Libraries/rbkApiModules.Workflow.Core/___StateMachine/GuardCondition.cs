﻿namespace Stateless;

internal class GuardCondition
    {
        Reflection.InvocationInfo _methodDescription;

        internal GuardCondition(Func<object[], bool> guard, Reflection.InvocationInfo description)
        {
            Guard = guard ?? throw new ArgumentNullException(nameof(guard));

            _methodDescription = description ?? throw new ArgumentNullException(nameof(description));
        }

        internal Func<object[], bool> Guard { get; }

        // Return the description of the guard method: the caller-defined description if one
        // was provided, else the name of the method itself
        internal string Description => _methodDescription.Description;

        // Return a more complete description of the guard method
        internal Reflection.InvocationInfo MethodDescription => _methodDescription;
    }