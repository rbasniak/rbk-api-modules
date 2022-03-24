using System;

namespace rbkApiModules.Utilities.EFCore
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class JsonColumnAttribute : Attribute 
    { 
    }
}
