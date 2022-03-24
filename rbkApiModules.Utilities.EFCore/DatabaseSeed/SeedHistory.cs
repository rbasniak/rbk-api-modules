using System;

namespace rbkApiModules.Utilities.EFCore
{
    /// <summary>
    /// Classe que armazena o históricos de que partes do seed foram aplicados ao banco de dados
    /// </summary>
    public class SeedHistory
    {

        protected SeedHistory()
        {

        }

        public SeedHistory(string id, DateTime dateApplied)
        {
            Id = id;
            DateApplied = dateApplied;
        }

        public virtual string Id { get; private set; }

        public virtual DateTime DateApplied { get; private set; }
    }
}