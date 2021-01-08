using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rbkApiModules.Workflow
{
    public class State : BaseEntity
    {
        private HashSet<Transition> _transitions;
        private HashSet<Transition> _usedBy;
        private HashSet<IStateEntity> _items;

        protected State()
        {

        }

        /// <summary>
        /// Método só deve ser utilizado para testes unitários.
        /// </summary>
        public State(Guid id, string name, string systemId, string color): this(name, systemId, color)
        {
            Id = id;
        }

        public State(string name, string systemId, string color)
        {
            _items = new HashSet<IStateEntity>();
            _transitions = new HashSet<Transition>();
            _usedBy = new HashSet<Transition>();

            Name = name;
            SystemId = systemId;
            Color = color;
        }

        public virtual string Name { get; private set; }

        /// <summary>
        /// Id interno do state, para ser utilizado nas partes em que as transições são hardcoded
        /// </summary>
        public virtual string SystemId { get; private set; }

        public virtual Guid? GroupId { get; private set; }
        public virtual StateGroup Group { get; private set; }

        /// <summary>
        /// Cor para o label no front
        /// </summary>
        public virtual string Color { get; private set; }

        /// <summary>
        /// Flag que indica se o state pode ou não ser apagado
        /// </summary>
        public virtual bool IsProtected => !String.IsNullOrEmpty(SystemId);

        public virtual IEnumerable<Transition> Transitions => _transitions?.ToList();
        public virtual IEnumerable<Transition> UsedBy => _usedBy?.ToList();
        public virtual IEnumerable<IStateEntity> Items => _items?.ToList();

        public Transition AddTransition(Event @event, State state, string history, bool isProtected)
        {
            var transition = new Transition(this, @event, state, history, isProtected);
            _transitions.Add(transition);

            return transition;
        }

        public void Update(string name, string color)
        {
            Name = name;
            Color = color;
        }

        public override string ToString()
        {
            return $"{Name} [{Transitions.Count()} transitions]";
        }
    }
}
