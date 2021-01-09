using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Workflow
{
    public class StateChangeEvent : BaseEntity
    {
        protected StateChangeEvent()
        {

        }

        public StateChangeEvent(IStateEntity entity, string username, State status, string history, string notes)
        {
            Entity = entity;
            Username = username;
            Date = DateTime.Now;
            StatusHistory = history;
            StatusId = status.Id;
            StatusName = status.Name;
            Notes = notes;
        }

        /// <summary>
        /// Texto com o nome do state que originou esse evento, no momento em que ele aconteceu
        /// </summary>
        public virtual string StatusName { get; private set; }

        /// <summary>
        /// Id do estado que originou esse evento, sem chave estrangeira no banco
        /// </summary>
        public virtual Guid StatusId { get; private set; }

        /// <summary>
        /// Texto para ser exibido no histórico da solicitação
        /// </summary>
        public virtual string StatusHistory { get; private set; }

        /// <summary>
        /// Chave do usário que provocou a mudança de estado
        /// </summary>
        public virtual string Username { get; private set; }

        public virtual DateTime Date { get; private set; }

        /// <summary>
        /// Solicitação de mudança à qual este evento pertence
        /// </summary>
        public virtual Guid EntityId { get; private set; }
        public virtual IStateEntity Entity { get; private set; }

        public virtual string Notes { get; private set; }

        public override string ToString()
        {
            return $"{Date.ToString()} - {StatusName}";
        }
    }
}
