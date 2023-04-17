using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Commons.Core;
using System.Reflection;

namespace rbkApiModules.Notifications.Core;

public static class Builder
{
    public static void AddRbkNotifications(this IServiceCollection services)
    {
        services.RegisterFluentValidators(Assembly.GetAssembly(typeof(GetNotifications.Request)));
    }
}
