using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChangeRequestWorkflow;

namespace Stateless
{
    internal class QueuedTrigger<TTrigger>
    {
        public TTrigger Trigger { get; set; }
        public object[] Args { get; set; }
    }
}
