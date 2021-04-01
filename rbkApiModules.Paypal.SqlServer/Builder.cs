using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Utilities.Extensions;
using System.Reflection;

namespace rbkApiModules.Paypal.SqlServer
{
    public static class Builder
    {
        public static void AddRbkApiPaypalModule<TPaypalActions>(this IServiceCollection services) where TPaypalActions : class, IPaypalActions
        {
            services.RegisterApplicationServices(Assembly.GetAssembly(typeof(IPaypalService)));

            services.AddTransient<IPaypalActions, TPaypalActions>();

            AssemblyScanner
                .FindValidatorsInAssembly(Assembly.GetAssembly(typeof(CreateWebhookEvent.Command)))
                    .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
        }
    }
}
