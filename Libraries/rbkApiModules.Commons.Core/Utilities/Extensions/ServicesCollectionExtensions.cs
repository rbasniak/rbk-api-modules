using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;
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
                    Log.Logger.Debug($"Registering validator: {result.InterfaceType.FullName}");

                    services.AddScoped(result.InterfaceType, result.ValidatorType);
                }
                else
                {
                    Log.Logger.Debug($"Skipping validator: {result.InterfaceType.FullName}");
                }    
            });
    }
}

public class IgnoreAutomaticIocContainerRegistrationAttribute: Attribute
{

}