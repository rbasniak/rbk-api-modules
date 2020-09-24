using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Utilities.Extensions;
using System.Reflection;

namespace rbkApiModules.Infrastructure.MediatR
{
    public static class BuilderExtensions
    {
        public static void AddRbkApiMediatRModule(this IServiceCollection services, Assembly[] assembliesForMediatR)
        {
            foreach (var assembly in assembliesForMediatR)
            {
                AssemblyScanner
                    .FindValidatorsInAssembly(assembly)
                    .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
            }

            services.RegisterApplicationServices(Assembly.GetAssembly(typeof(BuilderExtensions)));

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FailFastRequestBehavior<,>));

            services.AddMediatR(assembliesForMediatR);
        }
    }
}
