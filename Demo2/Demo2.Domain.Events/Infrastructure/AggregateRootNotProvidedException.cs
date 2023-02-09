using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.Infrastructure;

public class AggregateRootNotProvidedException : Exception
{
    public AggregateRootNotProvidedException(string message) : base(message)
    {

    }

}