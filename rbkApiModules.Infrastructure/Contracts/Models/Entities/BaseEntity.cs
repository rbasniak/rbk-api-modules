using System;

namespace rbkApiModules.Infrastructure
{
    /// <summary>
    /// Clase base para entidades do banco de dados
    /// </summary>
    public class BaseEntity
    {
        protected BaseEntity()
        {

        }

        /// <summary>
        /// Id da entidade
        /// </summary>
        public virtual Guid Id { get; set; }
    }
}
