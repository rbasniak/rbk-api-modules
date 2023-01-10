using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace rbkApiModules.Commons.Core.UiDefinitions;

public static class CoreUiDefinitionsBuilder
{
    public static void AddRbkUIDefinitions(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddSingleton(new UIDefinitionOptions(assemblies));
    }
}
