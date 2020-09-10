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
            services.RegisterApplicationServices(Assembly.GetAssembly(typeof(BuilderExtensions)));

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FailFastRequestBehavior<,>));

            services.AddMediatR(assembliesForMediatR);
        }
    }
}
