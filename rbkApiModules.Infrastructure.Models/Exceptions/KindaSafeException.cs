using System;

namespace rbkApiModules.Infrastructure.Models
{
    /// <summary>
    /// Classe para exceções "seguras" que são tratadas no código, armazenadas no banco de diagnostico e devem retornar para o cliente como erro 400
    /// </summary>
    public class KindaSafeException : ApplicationException
    {
        public KindaSafeException() : base()
        {
        }

        public KindaSafeException(string message) : base(message)
        {
        }

        public KindaSafeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
