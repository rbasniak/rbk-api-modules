using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.CodeGeneration
{
    public class TypescriptMethod
    {
        public TypescriptMethod()
        {

        }

        public string Name { get; set; }
        public TypescriptProperty ReturnType { get; set; }
        public List<TypescriptProperty> Parameters { get; set; }
    }
}
