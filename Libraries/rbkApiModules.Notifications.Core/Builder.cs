using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace rbkApiModules.Notifications.Core;

public static class Builder
{
    public static void AddRbkNotifications(this IServiceCollection services)
    {
        AssemblyScanner
            .FindValidatorsInAssembly(Assembly.GetAssembly(typeof(GetNotifications.Request)))
                .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
    }
}
