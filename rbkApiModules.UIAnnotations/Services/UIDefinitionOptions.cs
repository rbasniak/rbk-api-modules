using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace rbkApiModules.UIAnnotations
{
    public class UIDefinitionOptions
    {
        public UIDefinitionOptions(Assembly[] assemblies)
        {
            Assemblies = assemblies;
        }

        public Assembly[] Assemblies { get; set; }
    }
}
