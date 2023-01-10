using Microsoft.Extensions.DependencyInjection;

namespace rbkApiModules.Commons.Core.CQRS;

public static class Builder
{
    // TODO: Mover para o options geral
    public static IServiceCollection AddRbkInMemoryCqrsStore(this IServiceCollection services)
    {
        services.AddScoped<ICqrsReadStore, CqrsInMemoryStore>();

        return services;
    }
}
