using Microsoft.Extensions.DependencyInjection;
using Serilog;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CQRS;
using rbkApiModules.Commons.Relational.CQRS;
using rbkApiModules.Commons.Core.Pipelines;

namespace rbkApiModules.Commons.Relational;

public static class CommonsRelationalBuilder
{
    public static void AddRbkApiRelationalSetup(this IServiceCollection services, Action<RbkApiCoreOptions> configureOptions)
    {
        var options = new RbkApiCoreOptions();
        configureOptions(options);

        Log.Logger.Debug($"Start configuring Core Relational API capabilities");

        if (options._pipelines.Any(x => x == typeof(TransactionBehavior<,>)))
        {
            Log.Logger.Debug($"TransactionBehavior is being used, so DatabaseTransactionHandler will be enabled and this requires a valid DbContext in the project");

            services.AddTransient<IDatabaseTransactionHandler, DatabaseTransactionHandler>();
        }


        Log.Logger.Debug($"Done configuring Core Relational API capabilities");

        services.AddRbkApiCoreSetup(options);
    } 
}