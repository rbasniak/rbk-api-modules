using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public class StateGroup: BaseEntity
    {
        private HashSet<State> _states;

        protected StateGroup()
        {

        }

        public StateGroup(string name)
        {
            Name = name;

            _states = new HashSet<State>();
        }

        public virtual string Name { get; private set; }

        public virtual IEnumerable<State> States => _states?.ToList();

        public void Update(string name)
        {
            Name = name;
        }
    }
}
