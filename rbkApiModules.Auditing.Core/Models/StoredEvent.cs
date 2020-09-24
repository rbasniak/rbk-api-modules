using MediatR;
using System;

namespace rbkApiModules.Auditing.Core
{
    /// <summary>
    /// Evento de auditoria a ser salvo no banco de dados
    /// </summary>
    public class StoredEvent : IRequest
    {
        public StoredEvent(Guid? entityId, string type, string data, string user)
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
            EntityId = entityId;
            MessageType = type;
            Data = data;
            User = user;
        }

        /// <summary>
        /// EF Core constructor
        /// </summary>
        protected StoredEvent()
        {
        }

        /// <summary>
        /// Id do evento
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Data do evento
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Tipo do evento (nome da classe do comando que o originou)
        /// </summary>
        public string MessageType { get; protected set; }

        /// <summary>
        /// Id da entidade criada pelo comando
        /// </summary>
        public Guid? EntityId { get; protected set; }

        /// <summary>
        /// Nome do usuário que requisitou o comando
        /// </summary>
        public string User { get; private set; }

        /// <summary>
        /// Comando serializado para string
        /// </summary>
        public string Data { get; private set; }
    }
}