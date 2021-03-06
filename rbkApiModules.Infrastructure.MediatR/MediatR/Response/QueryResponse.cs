﻿namespace rbkApiModules.Infrastructure.MediatR.Core
{
    /// <summary>
    /// Classe gnérica de resposta de queries realizadas através do MediatR
    /// </summary>
    public class QueryResponse : BaseResponse
    {
        /// <summary>
        /// Construtor padrão
        /// </summary>
        public QueryResponse() : base()
        {
        }

        /// <summary>
        /// Construtor para comandos com resultado
        /// </summary>
        /// <param name="result">Resultado do comando</param>
        public QueryResponse(object result) : base(result)
        {
        }
    }
}
