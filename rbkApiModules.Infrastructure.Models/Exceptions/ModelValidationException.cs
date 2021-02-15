using System;

namespace rbkApiModules.Infrastructure.Models
{
    /// <summary>
    /// Classe para exceções "seguras" que foram origadas de validações de modelo
    /// </summary>
    public class ModelValidationException : ApplicationException
    {
        public ModelValidationException(ValidationResult[] errors): base("Erro de validação dos dados")
        {
            Errors = errors;
        }

        public ValidationResult[] Errors { get; private set; }
    }
}
