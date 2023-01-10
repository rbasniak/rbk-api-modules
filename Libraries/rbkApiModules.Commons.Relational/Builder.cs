using Microsoft.Extensions.DependencyInjection;
using Serilog;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CQRS;
using rbkApiModules.Commons.Relational.CQRS;

namespace rbkApiModules.Commons.Relational;

public static class CommonsRelationalBuilder
{
    public static void AddRbkApiRelationalSetup(this IServiceCollection services, Action<RbkApiCoreOptions> configureOptions)
    {
        var options = new RbkApiCoreOptions();
        configureOptions(options);

        Log.Logger.Debug($"Start configuring Core Relation API capabilities");

        services.AddTransient<IDatabaseTransactionHandler, DatabaseTransactionHandler>();

        Log.Logger.Debug($"Done configuring Core Relational API capabilities");

        services.AddRbkApiCoreSetup(options);
    }

    public static IServiceCollection AddRbkRelationalCqrsStore(this IServiceCollection services)
    {
        services.AddScoped<ICqrsReadStore, CqrsRelationalStore>();

        return services;
    }
}