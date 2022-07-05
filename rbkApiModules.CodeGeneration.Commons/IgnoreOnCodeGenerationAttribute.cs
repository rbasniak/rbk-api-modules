using System;

namespace rbkApiModules.CodeGeneration.Commons
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class IgnoreOnCodeGenerationAttribute: Attribute
    {
        public IgnoreOnCodeGenerationAttribute(IgnoreMode type)
        {
            Type = type;
        }

        public IgnoreOnCodeGenerationAttribute()
        {
            Type = IgnoreMode.All;
        }

        public IgnoreMode Type { get; }
    }

    public enum IgnoreMode
    {
        All,
        StateOnly,
        None,
    }
}
