using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rbkApiModules.Workflow
{
    public class Event : BaseEntity
    {
        protected HashSet<ClaimToEvent> _claims;
        protected HashSet<Transition> _transitions;

        protected Event()
        {

        }

        public Event(string name, string systemId, bool isActive)
        {
            _claims = new HashSet<ClaimToEvent>();
            _transitions = new HashSet<Transition>();

            Name = name;
            SystemId = systemId;
            IsActive = isActive;
        }

        /// <summary>
        /// Construtor para ser usado apenas em testes unitários
        /// </summary>
        public Event(Guid id, string name, string systemId, bool isActive) : this(name, systemId, isActive)
        {
            Id = id;
        }

        public virtual string Name { get; private set; }

        public virtual string SystemId { get; private set; }

        public virtual bool IsActive { get; private set; }

        public virtual bool IsProtected => !String.IsNullOrEmpty(SystemId);

        public virtual IEnumerable<Transition> Transitions => _transitions?.ToList();

        public virtual IEnumerable<ClaimToEvent> Claims => _claims?.ToList();

        public void Update(string name, bool isActive)
        {
            Name = name;
            IsActive = isActive;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
