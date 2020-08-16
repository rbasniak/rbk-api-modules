using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace rbkApiModules.Infrastructure
{
    /// <summary>
    /// Classe gnérica de resposta dos comandos do MediatR
    /// </summary>
    public class BaseResponse
    {
        private readonly IList<string> _messages = new List<string>();

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public BaseResponse()
        {
            Errors = new ReadOnlyCollection<string>(_messages);
        }

        /// <summary>
        /// Construtor para comandos com resultado
        /// </summary>
        /// <param name="result">Resultado do comando</param>
        public BaseResponse(object result) : this()
        {
            Result = result;
        }

        /// <summary>
        /// Status de válido do comando
        /// </summary>
        public bool IsValid => Status == CommandStatus.Valid;

        /// <summary>
        /// Lista de erros, caso o comando não seja válido
        /// </summary>
        public IEnumerable<string> Errors { get; }

        /// <summary>
        /// Objeto criado pela execução do comando
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// Status do comando
        /// </summary>
        public CommandStatus Status { get; set; }

        /// <summary>
        /// Adiciona um erro tratado na lista de erros (ex. validação)
        /// </summary>
        /// <param name="message">Texto com o erro</param>
        public void AddUnhandledError(string message)
        {
            Status = CommandStatus.HasUnhandledError;

            AddError(message);
        }

        /// <summary>
        /// Adiciona um erro não tratado na lista de erros (ex. exceção)
        /// </summary>
        /// <param name="message">Texto com o erro</param>
        public void AddHandledError(string message)
        {
            if (Status != CommandStatus.HasUnhandledError)
            {
                Status = CommandStatus.HasHandledError;
            }

            AddError(message);
        }

        /// <summary>
        /// Adiciona um erro à lista de erros
        /// </summary>
        /// <param name="message">Texto com o erro</param>
        private void AddError(string message)
        {
            _messages.Add(message);
        }
    }
}
