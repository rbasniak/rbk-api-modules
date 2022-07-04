using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.CodeGeneration
{
    public class PropertyInfo
    {
        public PropertyInfo(System.Reflection.PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;
            Type = new TypeInfo(propertyInfo.PropertyType);
        }

        public PropertyInfo(string name, Type type)
        {
            Name = name;
            Type = new TypeInfo(type);
        }

        public string Name { get; set; }
        public TypeInfo Type { get; set; }
    }


} 