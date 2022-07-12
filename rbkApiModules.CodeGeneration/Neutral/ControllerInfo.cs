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
    public class ControllerInfo
    {
        public ControllerInfo(Type type, string projectId)
        {
            Type = type;

            Name = type.FullName.Split('.').Last().Replace("Controller", String.Empty);
            var routeAttribute = type.GetAttribute<RouteAttribute>();
            Route = routeAttribute != null ? routeAttribute.Template.Replace("[controller]", Name.ToLower()) : String.Empty;

            if (type.HasAttribute<CodeGenerationScopeAttribute>())
            {
                var attribute = type.GetAttribute<CodeGenerationScopeAttribute>();

                Scopes = attribute.Scopes;
            }
            else
            {
                Scopes = new string[0];
            }

            Endpoints = GetEndpoints(type, projectId);
        }

        public Type Type { get; private set; }
        public string Name { get; private set; }
        public string[] Scopes { get; private set; }
        public string Route { get; private set; }
        public List<EndpointInfo> Endpoints { get; private set; }

        private List<EndpointInfo> GetEndpoints(Type controllerType, string projectId)
        {
            var methods = controllerType.GetMethods()
                .Where(x => x.GetCustomAttributes()
                    .Any(x => x.GetType().IsSubclassOf(typeof(HttpMethodAttribute))) && 
                        (x.GetCodeGenerationIgnoreMode() == IgnoreMode.None || x.GetCodeGenerationIgnoreMode() == IgnoreMode.StateOnly));

            var results = new List<EndpointInfo>();

            foreach (var action in methods)
            {
                var validScope = true;

                if (action.HasAttribute<CodeGenerationScopeAttribute>())
                {
                    var attribute = action.GetAttribute<CodeGenerationScopeAttribute>();

                    if (!attribute.Scopes.Any(x => x == projectId))
                    {
                        validScope = false;
                    }
                }
                else if (controllerType.HasAttribute<CodeGenerationScopeAttribute>())
                {
                    var attribute = controllerType.GetAttribute<CodeGenerationScopeAttribute>();

                    if (!attribute.Scopes.Any(x => x == projectId))
                    {
                        validScope = false;
                    }
                }

                if (validScope)
                {
                    results.Add(new EndpointInfo(this, action));
                }
            }

            return results;
        }
    }

} 