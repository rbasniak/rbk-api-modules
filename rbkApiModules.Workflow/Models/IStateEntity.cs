using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public interface IStateEntity
    {
        Guid StateId { get; }
        State State { get; }
        IEnumerable<StateChangeEvent> Events { get; }
    }
}
