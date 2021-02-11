using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace rbkApiModules.Utilities.Testing
{
    public sealed class AutoNamedFactAttribute : FactAttribute
    {
        public AutoNamedFactAttribute([CallerMemberName] string memberName = null)
        {
            DisplayName = memberName.Replace("_", " ");
        }
    }
}
