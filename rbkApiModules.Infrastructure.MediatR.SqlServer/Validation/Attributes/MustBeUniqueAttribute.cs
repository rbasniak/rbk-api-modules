using System;

namespace rbkApiModules.Infrastructure.MediatR.SqlServer
{
    public class MustBeUniqueAttribute : Attribute
    {
        public MustBeUniqueAttribute(Type type, string name)
        {
            EntityType = type;
            Name = name;
        }

        public Type EntityType { get; set; }

        public string Name { get; set; }
    }
}
