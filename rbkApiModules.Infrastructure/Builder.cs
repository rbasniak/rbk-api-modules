using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
