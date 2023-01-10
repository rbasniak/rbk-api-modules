using Stateless.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stateless
{
    /// <summary>
    /// Enum for the different modes used when Fire-ing a trigger
    /// </summary>
    public enum FiringMode
    {
        /// <summary> Use immediate mode when the queuing of trigger events are not needed. Care must be taken when using this mode, as there is no run-to-completion guaranteed.</summary>
        Immediate,
        /// <summary> Use the queued Fire-ing mode when run-to-completion is required. This is the recommended mode.</summary>
        Queued
    }
}
