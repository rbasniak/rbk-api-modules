using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Utilities.Extensions;
using System.Reflection;

namespace rbkApiModules.Notifications
{
    public static class Builder
    {
        public static void AddRbkApiNotificationsModule(this IServiceCollection services)
        {
            services.RegisterApplicationServices(Assembly.GetAssembly(typeof(INotificationsService)));

            AssemblyScanner
                .FindValidatorsInAssembly(Assembly.GetAssembly(typeof(GetNotifications.Command)))
                    .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
        }
    }
}
