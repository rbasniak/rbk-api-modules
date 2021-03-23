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
    public class SeedInfo
    {
        public Action<DbContext> Function { get; set; }
        public bool UseInProduction { get; set; }
    }
}