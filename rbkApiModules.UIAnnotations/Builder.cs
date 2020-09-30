using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace rbkApiModules.UIAnnotations
{
    public static class Builder
    {
        public static void AddRbkUIDefinitions(this IServiceCollection services, Assembly[] assemblies)
        {
            services.AddSingleton(new UIDefinitionOptions(assemblies));
        }
    }
}
