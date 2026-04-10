using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Features.ApplicationOptions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationOptionsServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationOptions<TOptions>(this IServiceCollection services)
        where TOptions : class, IApplicationOptions
    {
        // Register type so it can be discovered for seeding; do not register as singleton instance
        services.AddSingleton<IApplicationOptionsRegistration>(new ApplicationOptionsRegistration(typeof(TOptions)));
        return services;
    }

    public static IServiceCollection AddApplicationOptionsManager(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationOptionsManager, ApplicationOptionsManager>();
        return services;
    }
}

internal interface IApplicationOptionsRegistration
{
    Type OptionsType { get; }
}

internal sealed class ApplicationOptionsRegistration : IApplicationOptionsRegistration
{
    public ApplicationOptionsRegistration(Type optionsType)
    {
        OptionsType = optionsType;
    }

    public Type OptionsType { get; }
}

