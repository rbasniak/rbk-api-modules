using System;

namespace rbkApiModules.Infrastructure.Api
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class ApplicationAreaAttribute : Attribute
    {
        public ApplicationAreaAttribute(string area)
        {
            Area = area;
        }

        public string Area { get; }
    }
}
