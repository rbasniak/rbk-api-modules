using System;

namespace rbkApiModules.Infrastructure.MediatR.SqlServer
{
    public class MustExistAttribute : Attribute
    {
        public MustExistAttribute(Type type)
        {
            EntityType = type;
        }

        public Type EntityType { get; set; }
    }
}
