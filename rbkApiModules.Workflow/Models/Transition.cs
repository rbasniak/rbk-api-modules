using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Workflow
{
    public class Transition : BaseEntity
    {
        protected Transition()
        {

        }

        public Transition(State from, Event @event, State to, string history, bool isProtected)
        {
            FromState = from;
            ToState = to;
            Event = @event;
            History = history;
            IsProtected = isProtected;
        }

        /// <summary>
        /// State ao qual essa transição pertence
        /// </summary>
        public virtual Guid FromStateId { get; set; }
        public virtual State FromState { get; set; }

        /// <summary>
        /// State para o qual essa transição leva
        /// </summary>
        public virtual Guid ToStateId { get; set; }
        public virtual State ToState { get; set; }

        /// <summary>
        /// Evento que dispara essa transição
        /// </summary>
        public virtual Guid EventId { get; private set; }
        public virtual Event Event { get; private set; }

        /// <summary>
        /// Flag que indica se é possível ou não apagar essa transição
        /// </summary>
        public virtual bool IsProtected { get; private set; }

        /// <summary>
        /// Texto para ser exibido no histórico de operações da requisição
        /// </summary>
        public string History { get; private set; }

        public void Update(Event @event, State toState, string history)
        {
            Event = @event;
            ToState = toState;
            History = history;
        }

        public override string ToString()
        {
            return $"{FromState.Name} -> {Event.Name} -> {ToState.Name}";
        }
    }
}
