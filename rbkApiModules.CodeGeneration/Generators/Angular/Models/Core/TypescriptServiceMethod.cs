using rbkApiModules.CodeGeneration.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.CodeGeneration
{
    public class TypescriptServiceMethod : TypescriptMethod
    {
        public TypescriptServiceMethod(EndpointInfo endpoint)
        {
            Name = CodeGenerationUtilities.ToCamelCase(endpoint.Name);
            Route = endpoint.Route;
            Method = endpoint.Method.ToString().ToLower();

            if (endpoint.ReturnType != null)
            {
                ReturnType = new TypescriptProperty("", endpoint.ReturnType, false);
            }

            Parameters = new List<TypescriptProperty>();

            foreach (var parameter in endpoint.UrlParameters)
            {
                Parameters.Add(new TypescriptProperty(parameter.Name, parameter.Type, false));
            }

            if (endpoint.InputType != null)
            {
                Parameters.Add(new TypescriptProperty("data", endpoint.InputType, false));
            }
        }

        public string Route { get; set; }
        public string Method { get; set; }
    }
}
