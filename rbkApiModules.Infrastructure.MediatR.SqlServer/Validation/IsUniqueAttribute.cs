using System;

namespace rbkApiModules.Infrastructure.MediatR.SqlServer
{
    public class IsUniqueAttribute : Attribute
    {
        public IsUniqueAttribute(Type type, string name)
        {
            EntityType = type;
            Name = name;
        }

        public Type EntityType { get; set; }

        public string Name { get; set; }
    }
}
