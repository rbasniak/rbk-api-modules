using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Auditing.Relational;
using rbkApiModules.Commons.Core.Auditing;
using Serilog;

public static class Builder
{
    public static void AddRbkApiRelationalAuditing(this IServiceCollection services)
    {
        Log.Logger.Debug($"Start configuring relational auditing capabilities");

        services.AddTransient<ITraceLogStore, RelationalTraceLogStore>();

        Log.Logger.Debug($"Done configuring relational auditing capabilitiess");
    } 
}