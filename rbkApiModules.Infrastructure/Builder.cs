using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace rbkApiModules.Infrastructure
{
    public static class BuilderExtensions
    {
        public static void AddRbkApiModulesInfrastructure(this IServiceCollection services)
        {
            services.RegisterApplicationServices(Assembly.GetAssembly(typeof(BaseController)));
        }
    }
}
