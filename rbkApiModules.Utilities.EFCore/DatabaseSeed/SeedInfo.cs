using Microsoft.EntityFrameworkCore;
using System;

namespace rbkApiModules.Utilities.EFCore
{
    public class SeedInfo
    {
        public Action<DbContext> Function { get; set; }
        public bool UseInProduction { get; set; }
    }
}