using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Comments.Core;

namespace rbkApiModules.Comments.Relational;

public static class Builder
{
    public static IServiceCollection AddRbkRelationalComments(this IServiceCollection services, Action<RbkCommentsOptions> configureOptions = null)
    {
        var options = new RbkCommentsOptions();

        if (configureOptions != null)
        {
            configureOptions(options);
        }

        services.AddRbkCommentsCore(options);

        services.AddScoped<ICommentsService, RelationalCommentsService>();

        return services;
    }
}
