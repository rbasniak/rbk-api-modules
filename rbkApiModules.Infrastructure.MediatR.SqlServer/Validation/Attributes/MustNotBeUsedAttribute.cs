using System;

namespace rbkApiModules.Infrastructure.MediatR.SqlServer
{
    public class MustNotBeUsedAttribute : Attribute
    {
        public MustNotBeUsedAttribute()
        {
        }
    }
}
