using System;

namespace rbkApiModules.Infrastructure
{
    /// <summary>
    /// Classe para exceções "seguras" que são tratadas no código e devem retornar para o cliente
    /// </summary>
    public class SafeException : ApplicationException
    {
        public SafeException() : base()
        {
        }

        public SafeException(string message) : base(message)
        {
        }

        public SafeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
