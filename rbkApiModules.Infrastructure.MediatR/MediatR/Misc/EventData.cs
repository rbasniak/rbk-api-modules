using System;

namespace rbkApiModules.Infrastructure.MediatR.Core
{
    /// <summary>
    /// Classe com os dados de eventos que são salvos no bando de auditoria
    /// </summary>
    public class EventData
    {
        /// <summary>
        /// Id da entidade gerada pelo evento
        /// </summary>
        public Guid? EntityId { get; set; }

        /// <summary>
        /// Detalhes extras ppara serem salvos no banco (não implementado nesta versão)
        /// </summary>
        public object ExtraData { get; set; }
    }
}
