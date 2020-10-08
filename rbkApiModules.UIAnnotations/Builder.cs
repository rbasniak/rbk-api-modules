using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
