namespace rbkApiModules.Infrastructure
{
    /// <summary>
    /// Classe gnérica de resposta dos comandos do MediatR
    /// </summary>
    public class CommandResponse : BaseResponse
    {
        /// <summary>
        /// Construtor padrão
        /// </summary>
        public CommandResponse() : base()
        {
            EventData = new EventData();
        }

        /// <summary>
        /// Construtor para comandos com resultado
        /// </summary>
        /// <param name="result">Resultado do comando</param>
        public CommandResponse(object result) : base(result)
        {
        }

        /// <summary>
        /// Dados do evento para o banco de auditoria
        /// </summary>
        public EventData EventData { get; set; }
    }
}
