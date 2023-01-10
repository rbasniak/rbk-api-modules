using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Notifications.Core;

namespace rbkApiModules.Notifications.Relational;

public static class Builder
{
    public static void AddRbkRelationalNotifications(this IServiceCollection services)
    {
        services.AddRbkNotifications();

        services.AddScoped<INotificationsService, RelationalNotificationsService>();
    }
}
