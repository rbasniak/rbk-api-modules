using System;

namespace rbkApiModules.CodeGeneration.Commons
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class NgxsDatabaseStoreAttribute : Attribute
    {
        public NgxsDatabaseStoreAttribute(StoreType type)
        {

        }

        public StoreType Type { get; set; }
    }

    public enum StoreType
    {
        Readonly,
        Complete,
        Split
    }
}
