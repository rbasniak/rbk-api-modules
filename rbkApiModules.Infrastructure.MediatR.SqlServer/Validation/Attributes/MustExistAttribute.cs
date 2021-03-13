using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
