using FluentValidation;
using rbkApiModules.Commons.Core;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesCollectionExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var classTypes = assembly.ExportedTypes.Select(x => x.GetTypeInfo()).Where(x => x.IsClass && !x.IsAbstract);

            foreach (var type in classTypes.Where(x => !x.IsNested))
            {
                var interfaces = type.ImplementedInterfaces.Select(i => i.GetTypeInfo());

                // foreach (var handlerType in interfaces.Where(i => i.IsGenericType))
                foreach (var handlerType in interfaces)
                {
                    if (handlerType.Name == $"I{type.Name}" && !handlerType.HasAttribute<IgnoreAutomaticIocContainerRegistrationAttribute>())
                    {
                        if (services.None(x => x.ServiceType == handlerType.AsType() && x.ImplementationType == type.AsType()))
                        {
                            services.AddScoped(handlerType.AsType(), type.AsType());
                        }
                    }
                }
            }
        }
    }

    public static void RegisterFluentValidators(this IServiceCollection services, Assembly assembly)
    {
        AssemblyScanner
            .FindValidatorsInAssembly(assembly)
            .ForEach(result =>
            {
                var notRegistered = services.None(x => x.ServiceType != null && x.ServiceType == result.InterfaceType && x.ImplementationType == result.ValidatorType);

                if (notRegistered)
                {
                    services.AddScoped(result.InterfaceType, result.ValidatorType);
                }
                else
                {
                }    
            });
    }
}

public class IgnoreAutomaticIocContainerRegistrationAttribute: Attribute
{

}