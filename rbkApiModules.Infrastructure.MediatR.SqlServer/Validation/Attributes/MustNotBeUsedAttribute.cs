using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.MediatR.SqlServer
{
    public class MustNotBeUsedAttribute : Attribute
    {
        public MustNotBeUsedAttribute()
        {
        }
    }
}
