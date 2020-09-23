using System;

namespace rbkApiModules.Analytics.Core
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ApplicationAreaAttribute : Attribute
    {
        public ApplicationAreaAttribute(string area)
        {
            Area = area;
        }

        public string Area { get; }
    }
}
