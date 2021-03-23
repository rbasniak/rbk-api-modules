using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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