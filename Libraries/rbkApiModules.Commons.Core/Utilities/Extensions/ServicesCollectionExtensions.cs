using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace rbkApiModules.Commons.Core;

public static class ServicesCollectionExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var classTypes = assembly.ExportedTypes.Select(t => t.GetTypeInfo()).Where(t => t.IsClass && !t.IsAbstract);

            foreach (var type in classTypes.Where(x => !x.IsNested))
            {
                var interfaces = type.ImplementedInterfaces.Select(i => i.GetTypeInfo());

                // foreach (var handlerType in interfaces.Where(i => i.IsGenericType))
                foreach (var handlerType in interfaces)
                {
                    if (handlerType.Name == $"I{type.Name}" && !handlerType.HasAttribute<IgnoreAutomaticIocContainerRegistrationAttribute>())
                    {
                        services.AddScoped(handlerType.AsType(), type.AsType());
                    }
                }
            }
        }
    }
}

public class IgnoreAutomaticIocContainerRegistrationAttribute: Attribute
{

}